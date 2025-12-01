using GroundShare.BL;
using GroundShare.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace GroundShare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        // שליפת כל האירועים לפיד הראשי
        [HttpGet("all")]
        public IActionResult GetAllEvents()
        {
            try
            {
                EventsDAL dal = new EventsDAL();
                List<Event> events = dal.GetAllEvents();

                return Ok(events);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while fetching events");
            }
        }

        // שליפת אירוע בודד לפי ID
        [HttpGet("{id}")]
        public IActionResult GetEventById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid event id");
                }

                EventsDAL dal = new EventsDAL();
                Event ev = dal.GetEventById(id);

                if (ev == null)
                {
                    return NotFound("Event not found");
                }

                return Ok(ev);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while fetching event");
            }
        }

        // יצירת אירוע חדש
        [HttpPost("create")]
        public IActionResult CreateEvent([FromBody] Event ev)
        {
            try
            {
                if (ev == null)
                {
                    return BadRequest("Event data is null");
                }

                // בדיקות בסיסיות על שדות חובה
                if (ev.StartDateTime == default
                    || string.IsNullOrWhiteSpace(ev.EventsType)
                    || string.IsNullOrWhiteSpace(ev.Description)
                    || string.IsNullOrWhiteSpace(ev.Municipality)
                    || string.IsNullOrWhiteSpace(ev.ResponsibleBody)
                    || string.IsNullOrWhiteSpace(ev.EventsStatus)
                    || ev.LocationsId <= 0)
                {
                    return BadRequest("One or more required fields are missing or invalid");
                }

                EventsDAL dal = new EventsDAL();
                int newId = dal.CreateEvent(ev);

                if (newId <= 0)
                {
                    return StatusCode(500, "Failed to create event");
                }

                ev.EventsId = newId;

                return Ok(new
                {
                    EventsId = newId,
                    Message = "Event created successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while creating event");
            }
        }

        // עדכון אירוע קיים – מעדכן רק EventsType, Description, EventsStatus
        [HttpPut("update/{id}")]
        public IActionResult UpdateEvent(int id, [FromBody] Event ev)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid event id");
                }

                if (ev == null)
                {
                    return BadRequest("Event data is null");
                }

                // לוודא שה־EventsId בגוף מתאים ל־id ב־Route (אם בכלל הגיע)
                ev.EventsId = id;

                if (string.IsNullOrWhiteSpace(ev.EventsType)
                    || string.IsNullOrWhiteSpace(ev.Description)
                    || string.IsNullOrWhiteSpace(ev.EventsStatus))
                {
                    return BadRequest("One or more required fields are missing");
                }

                EventsDAL dal = new EventsDAL();
                bool updated = dal.UpdateEvent(ev);

                if (!updated)
                {
                    return NotFound("Event not found or not updated");
                }

                return Ok(new
                {
                    EventsId = id,
                    Message = "Event updated successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while updating event");
            }
        }

        // מחיקת אירוע + מחיקת כל הדירוגים שלו
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteEvent(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid event id");
                }

                EventsDAL dal = new EventsDAL();
                bool deleted = dal.DeleteEvent(id);

                if (!deleted)
                {
                    return NotFound("Event not found or already deleted");
                }

                return Ok(new
                {
                    EventsId = id,
                    Message = "Event deleted successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while deleting event");
            }
        }
    }
}