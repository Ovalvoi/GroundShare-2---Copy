using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class EventsDAL : DBServices
    {
        private SqlDataReader reader;
        private SqlConnection connection;
        private SqlCommand command;


        // ----------------------------------------------------
        // 1. שליפת אירוע בודד לפי ID - SP: spGetEventById
        // מחזיר אובייקט Event אחד, כולל כתובת (City/Street/HouseNumber)
        // ----------------------------------------------------
        public Event GetEventById(int eventId)
        {
            Event ev = null;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@EventsId", eventId);

                command = CreateCommandWithStoredProcedure("spGetEventById", connection, paramDic);

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ev = new Event();

                    // שדות מטבלת Events
                    ev.EventsId = Convert.ToInt32(reader["EventsId"]);
                    ev.StartDateTime = Convert.ToDateTime(reader["StartDateTime"]);
                    ev.EndDateTime = reader["EndDateTime"] == DBNull.Value
                                         ? (DateTime?)null
                                         : Convert.ToDateTime(reader["EndDateTime"]);
                    ev.EventsType = reader["EventsType"].ToString();
                    ev.PhotoUrl = reader["PhotoUrl"].ToString();
                    ev.Description = reader["Description"].ToString();
                    ev.Municipality = reader["Municipality"].ToString();
                    ev.ResponsibleBody = reader["ResponsibleBody"].ToString();
                    ev.EventsStatus = reader["EventsStatus"].ToString();
                    ev.LocationsId = Convert.ToInt32(reader["LocationsId"]);

                    // שדות מיקום מטבלת Locations (מצטרפים ב-JOIN ב-SP)
                    ev.City = reader["City"].ToString();
                    ev.Street = reader["Street"].ToString();
                    ev.HouseNumber = reader["HouseNumber"].ToString();

                    // שדות דירוג לא מחושבים ב-SP הזה, נשאיר ברירת מחדל (0)
                    ev.AvgRating = 0;
                    ev.RatingCount = 0;
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return ev; // null אם לא נמצא אירוע
        }

        // ----------------------------------------------------
        // 2. יצירת אירוע חדש - SP: spCreateEvent
        // משתמשים רק בשדות של טבלת Events
        // מחזיר את ה-EventsId החדש
        // ----------------------------------------------------
        public int CreateEvent(Event ev)
        {
            int newId = -1;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@StartDateTime", ev.StartDateTime);
                paramDic.Add("@EndDateTime", (object?)ev.EndDateTime ?? DBNull.Value);
                paramDic.Add("@EventsType", ev.EventsType);
                paramDic.Add("@PhotoUrl", (object?)ev.PhotoUrl ?? DBNull.Value);
                paramDic.Add("@Description", ev.Description);
                paramDic.Add("@Municipality", ev.Municipality);
                paramDic.Add("@ResponsibleBody", ev.ResponsibleBody);
                paramDic.Add("@EventsStatus", ev.EventsStatus);
                paramDic.Add("@LocationsId", ev.LocationsId);

                command = CreateCommandWithStoredProcedure("spCreateEvent", connection, paramDic);

                object scalar = command.ExecuteScalar();

                if (scalar != null && scalar != DBNull.Value)
                {
                    if (scalar is decimal dec)
                        newId = (int)dec;
                    else
                        newId = Convert.ToInt32(scalar);
                }
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return newId;
        }

        // ----------------------------------------------------
        // 3. עדכון אירוע קיים - SP: spUpdateEvent
        // ה-SP מעדכן רק: EventsType, Description, EventsStatus
        // מחזיר true אם עודכנה לפחות שורה אחת
        // ----------------------------------------------------
        public bool UpdateEvent(Event ev)
        {
            int rowsAffected = 0;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@EventsId", ev.EventsId);
                paramDic.Add("@EventsType", ev.EventsType);
                paramDic.Add("@Description", ev.Description);
                paramDic.Add("@EventsStatus", ev.EventsStatus);

                command = CreateCommandWithStoredProcedure("spUpdateEvent", connection, paramDic);

                rowsAffected = command.ExecuteNonQuery();
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return rowsAffected > 0;
        }

        // ----------------------------------------------------
        // 4. מחיקת אירוע - SP: spDeleteEvent
        // ה-SP מוחק קודם את כל ה-Rating של האירוע ואז את האירוע עצמו
        // מחזיר true אם נמחקו שורות (כלומר היה אירוע כזה)
        // ----------------------------------------------------
        public bool DeleteEvent(int eventId)
        {
            int rowsAffected = 0;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@EventsId", eventId);

                command = CreateCommandWithStoredProcedure("spDeleteEvent", connection, paramDic);

                rowsAffected = command.ExecuteNonQuery();
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return rowsAffected > 0;
        }

        // ----------------------------------------------------
        // 5. שליפת כל האירועים לפיד הראשי - SP: spGetAllEvents
        // כאן ה-SP כבר מחזיר:
        //  - נתוני אירוע
        //  - נתוני כתובת (City/Street/HouseNumber)
        //  - ממוצע דירוגים AvgRating
        //  - מספר דירוגים RatingCount
        // לכן זה מתאים בול למחלקת Event שלך
        // ----------------------------------------------------
        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();

            try
            {
                connection = Connect();

                command = CreateCommandWithStoredProcedure("spGetAllEvents", connection, null);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Event ev = new Event();

                    ev.EventsId = Convert.ToInt32(reader["EventsId"]);
                    ev.StartDateTime = Convert.ToDateTime(reader["StartDateTime"]);
                    ev.EndDateTime = reader["EndDateTime"] == DBNull.Value
                                       ? (DateTime?)null
                                       : Convert.ToDateTime(reader["EndDateTime"]);
                    ev.EventsType = reader["EventsType"].ToString();
                    ev.PhotoUrl = reader["PhotoUrl"].ToString();
                    ev.Description = reader["Description"].ToString();
                    ev.EventsStatus = reader["EventsStatus"].ToString();
                    ev.Municipality = reader["Municipality"].ToString();
                    ev.ResponsibleBody = reader["ResponsibleBody"].ToString();

                    // שדות מיקום
                    ev.City = reader["City"].ToString();
                    ev.Street = reader["Street"].ToString();
                    ev.HouseNumber = reader["HouseNumber"].ToString();

                    // שדות דירוגים מחושבים ב-SP
                    ev.AvgRating = Convert.ToDouble(reader["AvgRating"]);
                    ev.RatingCount = Convert.ToInt32(reader["RatingCount"]);

                    events.Add(ev);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return events;
        }
    }
}