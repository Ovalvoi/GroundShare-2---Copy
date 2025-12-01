using GroundShare.BL;
using GroundShare.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        // הוספת מיקום חדש
        [HttpPost("add")]
        public IActionResult AddLocation([FromBody] Location location)
        {
            try
            {
                if (location == null)
                {
                    return BadRequest("Location data is null");
                }

                // בדיקת שדות חובה
                if (string.IsNullOrWhiteSpace(location.City) ||
                    string.IsNullOrWhiteSpace(location.Street) ||
                    string.IsNullOrWhiteSpace(location.HouseNumber) ||
                    string.IsNullOrWhiteSpace(location.HouseType))
                {
                    return BadRequest("One or more required fields are missing");
                }

                LocationsDAL dal = new LocationsDAL();
                int newId = dal.AddLocation(location);

                if (newId <= 0)
                {
                    return StatusCode(500, "Failed to add location");
                }

                location.LocationsId = newId;

                return Ok(new
                {
                    LocationsId = newId,
                    Message = "Location added successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while adding location");
            }
        }

        // שליפת כל המיקומים
        [HttpGet("all")]
        public IActionResult GetAllLocations()
        {
            try
            {
                LocationsDAL dal = new LocationsDAL();
                List<Location> locations = dal.GetAllLocations();

                return Ok(locations);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while fetching locations");
            }
        }
    }
}
