using iig_webapp_test.Entities;
using iig_webapp_test.Helpers;
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

        [Authorize]
        [HttpGet("getProfile")]
        public IActionResult GetById()
        {
            var authen_user = (User)HttpContext.Items["User"];
            var user = _userService.GetById(authen_user.UserId);
            return Ok(user);
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest model)
        {
            _userService.Register(model);
            return Ok(new { status = true, message = "User created" });
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdateProfile(UpdateProfileRequest model)
        {
            //_userService.UpdateProfile(id, model);
            var authen_user = (User)HttpContext.Items["User"];
            _userService.UpdateProfile(authen_user.UserId, model);
            return Ok(new { status = true, message = "User updated" });
        }
    }
}
