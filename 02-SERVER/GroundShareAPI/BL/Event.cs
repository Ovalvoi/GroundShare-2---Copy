using System;
using System.Collections.Generic;
using GroundShare.DAL;

namespace GroundShare.BL
{
    public class Event
    {
        public int EventsId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string EventsType { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public string Municipality { get; set; }
        public string ResponsibleBody { get; set; }
        public string EventsStatus { get; set; }
        public int LocationsId { get; set; }

        // שדות תצוגה/פיד
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public double AvgRating { get; set; }
        public int RatingCount { get; set; }

        public Event()
        {
        }

        // שליפת כל האירועים
        public static List<Event> GetAll()
        {
            EventsDAL dal = new EventsDAL();
            return dal.GetAllEvents();
        }

        // שליפת אירוע בודד לפי ID
        public static Event GetById(int id)
        {
            EventsDAL dal = new EventsDAL();
            return dal.GetEventById(id);
        }

        // יצירת אירוע חדש
        public int Create()
        {
            EventsDAL dal = new EventsDAL();
            int newId = dal.CreateEvent(this);
            this.EventsId = newId;
            return newId;
        }

        // עדכון אירוע קיים
        public bool Update()
        {
            EventsDAL dal = new EventsDAL();
            return dal.UpdateEvent(this);
        }

        // מחיקת אירוע לפי ID
        public static bool Delete(int id)
        {
            EventsDAL dal = new EventsDAL();
            return dal.DeleteEvent(id);
        }
    }
}
