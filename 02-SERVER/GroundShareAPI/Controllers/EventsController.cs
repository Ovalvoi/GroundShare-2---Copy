using GroundShare.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroundShare.Controllers
{
    // קונטרולר לניהול אירועים והעלאת קבצים
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        // משתנה לקבלת מידע על סביבת השרת (נתיבים וכו')
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        // ---------------------------------------------------------------------------------
        // שליפת כל האירועים (GET api/Events/all)
        // ---------------------------------------------------------------------------------
        [HttpGet("all")]
        public IActionResult GetAllEvents()
        {
            return Ok(Event.GetAll());
        }

        // ---------------------------------------------------------------------------------
        // שליפת אירוע לפי ID (GET api/Events/{id})
        // ---------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public IActionResult GetEventById(int id)
        {
            Event ev = Event.GetById(id);
            if (ev == null) return NotFound("Event not found");
            return Ok(ev);
        }

        // ---------------------------------------------------------------------------------
        // יצירת אירוע חדש (POST api/Events/create)
        // ---------------------------------------------------------------------------------
        [HttpPost("create")]
        public IActionResult CreateEvent([FromBody] Event ev)
        {
            if (ev == null) return BadRequest("Data is null");
            int id = ev.Create();
            if (id <= 0) return StatusCode(500, "Failed to create event");
            return Ok(new { EventsId = id, Message = "Created successfully" });
        }

        // ---------------------------------------------------------------------------------
        // מחיקת אירוע (DELETE api/Events/delete/{id})
        // ---------------------------------------------------------------------------------
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteEvent(int id)
        {
            if (Event.Delete(id))
                return Ok(new { Message = "Deleted successfully" });
            return NotFound("Event not found");
        }

        // ---------------------------------------------------------------------------------
        // העלאת תמונה (POST api/Events/upload)
        // שומר את הקובץ בתיקיית wwwroot/images ומחזיר את הנתיב היחסי
        // ---------------------------------------------------------------------------------
        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            try
            {
                // יצירת שם קובץ ייחודי באמצעות GUID
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // נתיב התיקייה בשרת
                string imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // יצירת התיקייה אם אינה קיימת
                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                // שמירת הקובץ הפיזי
                string filePath = Path.Combine(imagesFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // החזרת הנתיב היחסי לשמירה ב-DB
                return Ok(new { path = "images/" + fileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}