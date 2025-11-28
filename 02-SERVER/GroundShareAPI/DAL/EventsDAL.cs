using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class EventsDAL
    {
        private DBServices _db;

        public EventsDAL()
        {
            _db = new DBServices();
        }

        public DataTable GetAllEvents()
        {
            // No parameters needed for Get All
            return _db.ExecuteReader("spGetAllEvents", null);
        }

        public int InsertEvent(DateTime start, DateTime? end, string type, string photoUrl, string desc, string municipality, string body, string status, int locId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@StartDateTime", start },
                { "@EndDateTime", end },
                { "@EventsType", type },
                { "@PhotoUrl", photoUrl },
                { "@Description", desc },
                { "@Municipality", municipality },
                { "@ResponsibleBody", body },
                { "@EventsStatus", status },
                { "@LocationsId", locId }
            };

            return _db.ExecuteNonQuery("spCreateEvent", parameters);
        }

        public int UpdateEvent(int id, string type, string desc, string status)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@EventsId", id },
                { "@EventsType", type },
                { "@Description", desc },
                { "@EventsStatus", status }
            };

            return _db.ExecuteNonQuery("spUpdateEvent", parameters);
        }

        public int DeleteEvent(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@EventsId", id }
            };

            return _db.ExecuteNonQuery("spDeleteEvent", parameters);
        }
    }
}