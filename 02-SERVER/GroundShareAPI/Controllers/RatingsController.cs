using GroundShare.BL;
using GroundShare.DAL;
using Microsoft.AspNetCore.Mvc;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        // POST: api/Ratings
        [HttpPost]
        public IActionResult Post([FromBody] Rating rating)
        {
            if (rating == null) return BadRequest("Invalid Data");

            int res = rating.Insert();
            return Ok(res);
        }

        [HttpGet("event/{eventId}")]
        public IActionResult GetByEvent(int eventId)
        {
            RatingsDAL dal = new RatingsDAL();
            // Assuming you have GetRatingsByEvent in DAL (see below)
            var dt = dal.GetRatingsByEvent(eventId);

            // Convert DataTable to List of objects manually or via helper
            var list = new List<object>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                list.Add(new
                {
                    overallScore = dr["OverallScore"],
                    comment = dr["Comment"],
                    createdAt = dr["CreatedAt"]
                });
            }
            return Ok(list);
        }
    }
}