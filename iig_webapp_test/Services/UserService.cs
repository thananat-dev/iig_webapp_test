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

namespace iig_webapp_test.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        void Register(RegisterRequest model);
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        User? GetById(long id);
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
