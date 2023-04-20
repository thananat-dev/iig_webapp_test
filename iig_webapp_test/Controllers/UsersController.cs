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

        [HttpPost("authenticate")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { status = false, message = "Username or password is incorrect" });

            return Ok(response);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { status = true, message = "User created" });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(long id, UpdateProfileRequest model)
        {
            _userService.UpdateProfile(id, model);
            return Ok(new { status = true, message = "User updated" });
        }
    }
}
