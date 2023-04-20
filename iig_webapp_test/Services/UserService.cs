using AutoMapper;
using iig_webapp_test.Data;
using iig_webapp_test.Entities;
using iig_webapp_test.Helpers;
using iig_webapp_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace iig_webapp_test.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        void Register(RegisterRequest model);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        User? GetById(long id);
        void UpdateProfile(long userId, UpdateProfileRequest model);
    }
    public class UserService:IUserService
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserService(DatabaseContext db, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _db = db;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _db.Users.FirstOrDefault(x => x.Username == model.Username);

            // return null if user not found or password not match
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password)) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.ToList();
        }

        public User? GetById(long id)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserId == id);
            // validate
            if (user == null)
                throw new AppException("User '" + user.UserId + "' not found");
            return user;
        }

        public void Register(RegisterRequest model)
        {
            //if(IsPasswordValid(model.Password))
            //    throw new AppException("Password must not be a sequence of letters or numbers.");
            // validate
            if (_db.Users.Any(x => x.Username == model.Username))
                throw new AppException("User with the Username '" + model.Username + "' already exists");

            // map model to new user object
            var user = _mapper.Map<User>(model);

            // hash password
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // save user
            _db.Users.Add(user);
            _db.SaveChanges();
        }

        public static bool IsPasswordValid(string password)
        {
            bool isValid = Regex.IsMatch(password, @"^(?=.*[a-zA-Z])(?=.*\d)(?!.*(.)\1{2}).{6,}$");
            return !isValid;
        }

        private static bool IsPasswordSequential(string password)
        {
            bool isSequential = false;
            for (int i = 0; i < password.Length - 1; i++)
            {
                if (char.IsDigit(password[i]) && char.IsDigit(password[i + 1]))
                {
                    int currentNumber = int.Parse(password[i].ToString());
                    int nextNumber = int.Parse(password[i + 1].ToString());
                    if (nextNumber - currentNumber == 1 || nextNumber - currentNumber == -1)
                    {
                        isSequential = true;
                        break;
                    }
                }
                else if (char.IsLetter(password[i]) && char.IsLetter(password[i + 1]))
                {
                    if ((password[i] + 1) == password[i + 1] || (password[i] - 1) == password[i + 1])
                    {
                        isSequential = true;
                        break;
                    }
                }
            }
            return isSequential;
        }


        public void UpdateProfile(long userId, UpdateProfileRequest model)
        {
            var user = _db.Users.FirstOrDefault(x => x.UserId == userId);

            // validate
            if (user == null)
                throw new AppException("User '" + model.FirstName + "' not found");

            // hash password if not empty
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (CheckOldFivePassword(userId, model.NewPassword, user.Password))
                    user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            }

            // copy model to user and save
            _mapper.Map(model, user);
            _db.Users.Update(user);
            _db.SaveChanges();
        }

        // Repeat the password check for the last 5 password changes.
        private bool CheckOldFivePassword(long userId, string newPassword, string oldPassword)
        {
            var changePasswordList = _db.ChangeUserPasswords.Where(x => x.UserId == userId).OrderBy(o => o.LastUpdate).ToList();
            if (changePasswordList.Any(x => BCrypt.Net.BCrypt.Verify(newPassword, x.UserOldPassword)) || BCrypt.Net.BCrypt.Verify(newPassword, oldPassword))
                throw new AppException("The password is the same as the last 5 passwords, please try again.");


            if (changePasswordList.Count == 4) // Including the last password will equal 5 password changes.
            {
                changePasswordList[0].UserOldPassword = oldPassword;
                changePasswordList[0].LastUpdate = DateTime.Now;
            }
            else
            {
                ChangeUserPassword changeUserPassword = new ChangeUserPassword();
                changeUserPassword.UserId = userId;
                changeUserPassword.UserOldPassword = oldPassword;
                changeUserPassword.LastUpdate = DateTime.Now;
                _db.ChangeUserPasswords.Add(changeUserPassword);
            }
            _db.SaveChanges();
            return true;
        }

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
