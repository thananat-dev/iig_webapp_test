using AutoMapper;
using iig_webapp_test.Data;
using iig_webapp_test.Entities;
using iig_webapp_test.Helpers;
using iig_webapp_test.Models;

namespace iig_webapp_test.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        void Register(RegisterRequest model);
    }
    public class UserService:IUserService
    {
        private readonly DatabaseContext _db;
        private readonly IMapper _mapper;

        public UserService(DatabaseContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.ToList();
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
    }
}
