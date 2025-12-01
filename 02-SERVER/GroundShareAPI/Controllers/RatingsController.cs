using GroundShare.BL;
using GroundShare.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        // הוספת דירוג חדש
        [HttpPost("add")]
        public IActionResult AddRating([FromBody] Rating rating)
        {
            try
            {
                if (rating == null)
                {
                    return BadRequest("Rating data is null");
                }

                if (rating.UserId <= 0 ||
                    rating.EventsId <= 0)
                {
                    return BadRequest("UserId or EventId is missing");
                }

                RatingsDAL dal = new RatingsDAL();
                int newId = dal.AddRating(rating);

                if (newId <= 0)
                {
                    return StatusCode(500, "Failed to insert rating");
                }

                rating.RatingId = newId;

                return Ok(new
                {
                    RatingId = newId,
                    Message = "Rating added successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while adding rating");
            }
        }

        // שליפת כל הדירוגים של אירוע
        [HttpGet("byEvent/{eventId}")]
        public IActionResult GetRatingsByEvent(int eventId)
        {
            try
            {
                if (eventId <= 0)
                {
                    return BadRequest("Invalid eventId");
                }

                RatingsDAL dal = new RatingsDAL();
                List<Rating> ratings = dal.GetRatingsByEvent(eventId);

                return Ok(ratings);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while fetching ratings");
            }
        }
    }
}
