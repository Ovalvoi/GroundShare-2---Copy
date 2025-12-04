using GroundShare.BL;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GroundShare.Controllers
{
    // קונטרולר לניהול מיקומים
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        // ---------------------------------------------------------------------------------
        // הוספת מיקום (POST api/Locations/add)
        // ---------------------------------------------------------------------------------
        [HttpPost("add")]
        public IActionResult AddLocation([FromBody] Location location)
        {
            if (location == null) return BadRequest("Data null");
            int id = location.Add();
            if (id <= 0) return StatusCode(500, "Failed to add");
            return Ok(new { LocationsId = id, Message = "Added" });
        }

        // ---------------------------------------------------------------------------------
        // שליפת כל המיקומים (GET api/Locations/all)
        // ---------------------------------------------------------------------------------
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            return Ok(Location.GetAll());
        }
    }
}