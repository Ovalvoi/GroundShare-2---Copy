using GroundShare.BL;
using Microsoft.AspNetCore.Mvc;

namespace GroundShare.Controllers
{
    // קונטרולר לניהול דירוגים
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        // ---------------------------------------------------------------------------------
        // הוספת דירוג חדש (POST api/Ratings/add)
        // ---------------------------------------------------------------------------------
        [HttpPost("add")]
        public IActionResult AddRating([FromBody] Rating rating)
        {
            // ולידציה בסיסית - חובה שיהיה מזהה משתמש ומזהה אירוע
            if (rating == null || rating.UserId <= 0 || rating.EventsId <= 0)
                return BadRequest("Invalid data");

            int id = rating.Add();
            if (id <= 0) return StatusCode(500, "Failed to add rating");

            return Ok(new { RatingId = id, Message = "Added successfully" });
        }

        // ---------------------------------------------------------------------------------
        // שליפת דירוגים לאירוע מסוים (GET api/Ratings/byEvent/{eventId})
        // ---------------------------------------------------------------------------------
        [HttpGet("byEvent/{eventId}")]
        public IActionResult GetByEvent(int eventId)
        {
            return Ok(Rating.GetByEvent(eventId));
        }
    }
}