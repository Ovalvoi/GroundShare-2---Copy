using System;
using System.Collections.Generic;
using GroundShare.DAL;

namespace GroundShare.BL
{
    // מחלקה המייצגת אירוע במערכת (Business Logic)
    public class Event
    {
        // מאפיינים התואמים לטבלת Events
        public int EventsId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; } // יכול להיות ריק (Null)
        public string? EventsType { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Description { get; set; }
        public string? Municipality { get; set; }
        public string? ResponsibleBody { get; set; }
        public string? EventsStatus { get; set; }
        public int LocationsId { get; set; }

        // מאפיינים נוספים לתצוגה שמגיעים מטבלאות מקושרות (JOIN) או חישובים
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public double AvgRating { get; set; } // ממוצע דירוג
        public int RatingCount { get; set; }  // כמות מדרגים

        public Event() { }

        // ---------------------------------------------------------------------------------
        // שליפת כל האירועים (Static Method)
        // פונקציה סטטית כי היא לא תלויה במופע ספציפי של אירוע
        // ---------------------------------------------------------------------------------
        public static List<Event> GetAll()
        {
            EventsDAL dal = new EventsDAL();
            return dal.GetAllEvents();
        }

        // ---------------------------------------------------------------------------------
        // שליפת אירוע בודד לפי ID (Static Method)
        // ---------------------------------------------------------------------------------
        public static Event GetById(int id)
        {
            EventsDAL dal = new EventsDAL();
            return dal.GetEventById(id);
        }

        // ---------------------------------------------------------------------------------
        // יצירת אירוע חדש (Instance Method)
        // משתמשת בנתונים של האובייקט הנוכחי (this)
        // ---------------------------------------------------------------------------------
        public int Create()
        {
            EventsDAL dal = new EventsDAL();
            int id = dal.CreateEvent(this);
            this.EventsId = id; // עדכון ה-ID שנוצר באובייקט הנוכחי
            return id;
        }

        // ---------------------------------------------------------------------------------
        // מחיקת אירוע לפי ID (Static Method)
        // ---------------------------------------------------------------------------------
        public static bool Delete(int id)
        {
            EventsDAL dal = new EventsDAL();
            return dal.DeleteEvent(id);
        }
    }
}