using Microsoft.AspNetCore.Mvc;
using GroundShare.BL;
using System.Collections.Generic;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        // GET: api/Locations
        [HttpGet]
        public IEnumerable<Location> Get()
        {
            Location l = new Location();
            return l.Read();
        }

        // POST: api/Locations
        [HttpPost]
        public IActionResult Post([FromBody] Location location)
        {
            if (location == null) return BadRequest("Invalid Data");

            int res = location.Insert();
            return Ok(res);
        }
    }
}