using Microsoft.AspNetCore.Mvc;
using GroundShare.BL;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
            // ------------------------------------------------------
            // POST api/user/register
            // רישום משתמש חדש דרך user.Register()
            // ------------------------------------------------------
            [HttpPost("register")]
            public IActionResult Register([FromBody] User user)
            {
                try
                {
                    if (user == null)
                    {
                        return BadRequest("User data is null.");
                    }

                    // user.RegisteRegister();r() קורא ל-UsersDAL.RegisterUser(user)
                    int result = user.Register();

                    if (result == -1)
                    {
                        return Conflict("Email already exists.");
                    }

                    return Ok(new { UserId = result, Message = "User registered successfully." });
                }
                catch (Exception)
                {
                    return StatusCode(500, "Internal server error while registering user.");
                }
            }

        // ------------------------------------------------------
        //POST api/user/login
        //התחברות משתמש – מקבל User עם Email + Password בלבד
        // ------------------------------------------------------
            [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                if (user == null ||
                    string.IsNullOrWhiteSpace(user.Email) ||
                    string.IsNullOrWhiteSpace(user.Password))
                {
                    return BadRequest("Email or password is missing.");
                }

                // User.Login(email, password) → משתמש ב-UsersDAL.Login
                //User loggedUser = User.Login(user.Email, user.Password);
                User loggedUser = BL.User.Login(user.Email, user.Password);


                if (loggedUser == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                return Ok(loggedUser);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while logging in.");
            }
        }
    }
}