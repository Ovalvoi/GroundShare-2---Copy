using Microsoft.AspNetCore.Mvc;
using GroundShare.BL;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        // We inject the environment to get the path to wwwroot
        public EventsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Events
        [HttpGet]
        public IEnumerable<Event> Get()
        {
            Event e = new Event();
            return e.Read();
        }

        // POST: api/Events
        [HttpPost]
        public IActionResult Post([FromBody] Event eventObj)
        {
            if (eventObj == null) return BadRequest("Invalid Data");

            int res = eventObj.Insert();
            return Ok(res);
        }

        // PUT: api/Events/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Event eventObj)
        {
            eventObj.EventsId = id; // Ensure ID is set
            int res = eventObj.Update();

            if (res > 0) return Ok(res);
            return NotFound();
        }

        // DELETE: api/Events/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Event e = new Event();
            int res = e.Delete(id);

            if (res > 0) return Ok(res);
            return NotFound();
        }

        // --------------------------------------------------------
        // PART 5: FILE UPLOAD
        // POST: api/Events/upload
        // --------------------------------------------------------
        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                // 1. Generate a unique filename to prevent overwrites
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // 2. Get the path to wwwroot/images
                string imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // 3. Create directory if it doesn't exist
                if (!Directory.Exists(imagesFolder))
                {
                    Directory.CreateDirectory(imagesFolder);
                }

                // 4. Full path to save the file
                string filePath = Path.Combine(imagesFolder, fileName);

                // 5. Save the file stream
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // 6. Return the relative URL (path) that will be saved in the DB
                // This string is what the Client sends back in the "PhotoUrl" field
                string relativePath = "images/" + fileName;

                return Ok(new { path = relativePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}