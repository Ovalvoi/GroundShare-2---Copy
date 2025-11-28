using System;
using System.Collections.Generic;
using System.Data;
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
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public double AvgRating { get; set; }
        public int RatingCount { get; set; }

        public Event() { }

        // Get All
        public List<Event> Read()
        {
            EventsDAL dal = new EventsDAL();
            List<Event> eventsList = new List<Event>();

            DataTable dt = dal.GetAllEvents();

            foreach (DataRow dr in dt.Rows)
            {
                Event e = new Event();
                e.EventsId = Convert.ToInt32(dr["EventsId"]);
                e.StartDateTime = Convert.ToDateTime(dr["StartDateTime"]);
                if (dr["EndDateTime"] != DBNull.Value) e.EndDateTime = Convert.ToDateTime(dr["EndDateTime"]);
                e.EventsType = dr["EventsType"].ToString();
                e.PhotoUrl = dr["PhotoUrl"] != DBNull.Value ? dr["PhotoUrl"].ToString() : "";
                e.Description = dr["Description"] != DBNull.Value ? dr["Description"].ToString() : "";
                e.EventsStatus = dr["EventsStatus"].ToString();
                e.Municipality = dr["Municipality"].ToString();
                e.ResponsibleBody = dr["ResponsibleBody"].ToString();
                e.City = dr["City"].ToString();
                e.Street = dr["Street"].ToString();
                e.HouseNumber = dr["HouseNumber"].ToString();
                e.AvgRating = Convert.ToDouble(dr["AvgRating"]);
                e.RatingCount = Convert.ToInt32(dr["RatingCount"]);
                eventsList.Add(e);
            }
            return eventsList;
        }

        // Insert
        public int Insert()
        {
            EventsDAL dal = new EventsDAL();
            return dal.InsertEvent(this.StartDateTime, this.EndDateTime, this.EventsType, this.PhotoUrl, this.Description, this.Municipality, this.ResponsibleBody, this.EventsStatus, this.LocationsId);
        }

        // Update
        public int Update()
        {
            EventsDAL dal = new EventsDAL();
            return dal.UpdateEvent(this.EventsId, this.EventsType, this.Description, this.EventsStatus);
        }

        // Delete
        public int Delete(int id)
        {
            EventsDAL dal = new EventsDAL();
            return dal.DeleteEvent(id);
        }
    }
}