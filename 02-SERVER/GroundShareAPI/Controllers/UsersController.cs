using Microsoft.AspNetCore.Mvc;
using GroundShare.BL;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // ------------------------------------------------------
        // POST api/Users/login
        // התחברות לפי המבנה מהפרויקט העובד
        // ------------------------------------------------------
        [HttpPost("login")]
        public IActionResult Login([FromBody] User loginDetails)
        {
            // בדיקה בסיסית שאכן נשלחו נתונים
            if (loginDetails == null || string.IsNullOrEmpty(loginDetails.Email) || string.IsNullOrEmpty(loginDetails.Password))
            {
                return BadRequest("Email and Password are required.");
            }

            // יצירת מופע ריק כדי לקרוא לפונקציה (ממש כמו בדוגמה שלך)
            User userHelper = new User();
            User userFromServer = userHelper.Login(loginDetails.Email, loginDetails.Password);

            if (userFromServer != null)
            {
                return Ok(userFromServer);
            }

            return Unauthorized("Incorrect email or password"); // 401
        }

        // ------------------------------------------------------
        // POST api/Users/register
        // הרשמה לפי המבנה מהפרויקט העובד
        // ------------------------------------------------------
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (user == null) return BadRequest("No user data provided");

            // קריאה לפונקציה Insert של המודל
            int newId = user.Insert();

            if (newId == -1)
            {
                return Conflict("Email already exists"); // 409
            }

            // החזרת אובייקט עם המידע כדי שהקליינט יוכל להשתמש בזה
            return Ok(new { UserId = newId, Message = "User registered successfully" });
        }
    }
}