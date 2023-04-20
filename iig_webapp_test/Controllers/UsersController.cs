using iig_webapp_test.Entities;
using iig_webapp_test.Models;
using iig_webapp_test.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iig_webapp_test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { status = true, message = "User created" });
        }
    }
}
