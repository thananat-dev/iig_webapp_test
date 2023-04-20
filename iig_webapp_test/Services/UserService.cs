using iig_webapp_test.Data;
using iig_webapp_test.Entities;

namespace iig_webapp_test.Services
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
    }
    public class UserService:IUserService
    {
        private readonly DatabaseContext _db;

        public UserService(DatabaseContext db)
        {
            _db = db;
        }

        public IEnumerable<User> GetAll()
        {
            return _db.Users.ToList();
        }
    }
}
