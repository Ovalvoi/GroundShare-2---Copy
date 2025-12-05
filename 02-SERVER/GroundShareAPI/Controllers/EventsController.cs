using GroundShare.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;

namespace GroundShare.Controllers
{
    // קונטרולר לניהול כל הפעולות הקשורות לאירועים (Events)
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        // שדה פרטי לאחסון המידע על סביבת הריצה של השרת (web hosting environment).
        // משמש בעיקר כדי לקבוע את הנתיב הפיזי לשמירת קבצים (כמו תמונות).
        private readonly IWebHostEnvironment _webHostEnvironment;

        // בנאי (Constructor) של הקונטרולר.
        // מקבל את IWebHostEnvironment באמצעות הזרקת תלויות (Dependency Injection) ושומר אותו במשתנה המקומי.
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
            // קורא לפונקציה הסטטית GetAll מהמחלקה Event (שכבת BL)
            // ומחזיר את רשימת האירועים עם קוד סטטוס 200 (OK).
            return Ok(Event.GetAll());
        }

        // ---------------------------------------------------------------------------------
        // שליפת אירוע בודד לפי מזהה (GET api/Events/{id})
        // ---------------------------------------------------------------------------------
        [HttpGet("{id}")]
        public IActionResult GetEventById(int id)
        {
            // קורא לפונקציה הסטטית GetById מהמחלקה Event (שכבת BL)
            Event ev = Event.GetById(id);
            // אם לא נמצא אירוע עם המזהה שצוין, מחזיר 404 (Not Found).
            if (ev == null) return NotFound("Event not found");
            // אם האירוע נמצא, מחזיר אותו עם קוד סטטוס 200 (OK).
            return Ok(ev);
        }

        // ---------------------------------------------------------------------------------
        // יצירת אירוע חדש (POST api/Events/create)
        // ---------------------------------------------------------------------------------
        [HttpPost("create")]
        public IActionResult CreateEvent([FromBody] Event ev)
        {
            // בדיקה אם גוף הבקשה (body) ריק.
            if (ev == null) return BadRequest("Data is null");
            // קורא לפונקציית Create של אובייקט האירוע (שכבת BL).
            int id = ev.Create();
            // אם ה-ID שחזר קטן או שווה ל-0, סימן שהייתה שגיאה ביצירה בבסיס הנתונים.
            if (id <= 0) return StatusCode(500, "Failed to create event");
            // אם היצירה הצליחה, מחזיר את ה-ID החדש של האירוע עם הודעת הצלחה וקוד 200 (OK).
            return Ok(new { EventsId = id, Message = "Created successfully" });
        }

        // ---------------------------------------------------------------------------------
        // מחיקת אירוע לפי מזהה (DELETE api/Events/delete/{id})
        // ---------------------------------------------------------------------------------
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteEvent(int id)
        {
            // קורא לפונקציה הסטטית Delete מהמחלקה Event (שכבת BL).
            // אם הפונקציה מחזירה true, המחיקה הצליחה.
            if (Event.Delete(id))
                return Ok(new { Message = "Deleted successfully" });
            // אם הפונקציה מחזירה false, האירוע לא נמצא.
            return NotFound("Event not found");
        }

        // ---------------------------------------------------------------------------------
        // העלאת קובץ (תמונה) לשרת (POST api/Events/upload)
        // ---------------------------------------------------------------------------------
        [HttpPost("upload")]
        public IActionResult Upload(IFormFile file)
        {
            // בדיקה אם לא נשלח קובץ או שהקובץ ריק.
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            try
            {
                // יצירת שם ייחודי לקובץ כדי למנוע התנגשויות של שמות זהים.
                // Guid.NewGuid() יוצר מזהה ייחודי גלובלי.
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                // הגדרת הנתיב לתיקיית 'images' שנמצאת בתוך תיקיית השורש של השרת (wwwroot).
                string imagesFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                // אם התיקייה לא קיימת, יוצרים אותה.
                if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);
                // הרכבת הנתיב המלא לקובץ החדש.
                string filePath = Path.Combine(imagesFolder, fileName);
                // יצירת stream לקובץ החדש ושמירת תוכן הקובץ שהועלה לתוכו.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                // אם ההעלאה הצליחה, מחזירים את הנתיב היחסי לקובץ, כדי שהלקוח יוכל לשמור אותו בבסיס הנתונים.
                return Ok(new { path = "images/" + fileName });
            }
            catch (Exception ex)
            {
                // במקרה של שגיאה כלשהי בתהליך, מחזירים שגיאת שרת 500 עם פירוט.
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}