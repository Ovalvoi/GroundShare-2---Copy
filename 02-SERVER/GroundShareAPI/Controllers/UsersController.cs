using Microsoft.AspNetCore.Mvc;
using GroundShare.BL;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // POST api/Users/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] User loginDetails)
        {
            User user = new User();
            User userFromServer = user.Login(loginDetails.Email, loginDetails.Password);

            if (userFromServer != null)
            {
                return Ok(userFromServer);
            }
            return Unauthorized("Incorrect email or password"); // 401
        }

        // POST api/Users/register
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (user == null) return BadRequest("No user data provided");

            int newId = user.Insert();

            if (newId == -1)
            {
                return Conflict("Email already exists"); // 409
            }

            return Ok(newId);
        }
    }
}