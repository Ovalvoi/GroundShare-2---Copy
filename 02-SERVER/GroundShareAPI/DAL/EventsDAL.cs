using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    // שכבת הגישה לנתונים עבור אירועים
    public class EventsDAL : DBServices
    {
        // ---------------------------------------------------------------------------------
        // שליפת כל האירועים באמצעות spGetAllEvents
        // כולל מידע על מיקום ודירוגים (JOIN ב-SQL)
        // ---------------------------------------------------------------------------------
        public List<Event> GetAllEvents()
        {
            List<Event> list = new List<Event>();
            SqlConnection connection = null;
            SqlDataReader reader = null;

            try
            {
                connection = Connect();
                SqlCommand command = CreateCommandWithStoredProcedure("spGetAllEvents", connection, null);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Event ev = new Event();
                    // המרה בטוחה של נתונים מה-Reader
                    ev.EventsId = Convert.ToInt32(reader["EventsId"]);
                    ev.StartDateTime = Convert.ToDateTime(reader["StartDateTime"]);
                    // בדיקת DBNull עבור שדות שיכולים להיות ריקים
                    ev.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDateTime"]);
                    ev.EventsType = reader["EventsType"].ToString();
                    ev.PhotoUrl = reader["PhotoUrl"].ToString();
                    ev.Description = reader["Description"].ToString();
                    ev.EventsStatus = reader["EventsStatus"].ToString();
                    ev.Municipality = reader["Municipality"].ToString();
                    ev.ResponsibleBody = reader["ResponsibleBody"].ToString();

                    // נתוני המיקום
                    ev.City = reader["City"].ToString();
                    ev.Street = reader["Street"].ToString();
                    ev.HouseNumber = reader["HouseNumber"].ToString();

                    // נתוני הדירוג
                    ev.AvgRating = Convert.ToDouble(reader["AvgRating"]);
                    ev.RatingCount = Convert.ToInt32(reader["RatingCount"]);

                    list.Add(ev);
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (connection != null) connection.Close();
            }
            return list;
        }

        // ---------------------------------------------------------------------------------
        // שליפת אירוע בודד לפי מזהה
        // ---------------------------------------------------------------------------------
        public Event GetEventById(int id)
        {
            Event ev = null;
            SqlConnection connection = null;
            SqlDataReader reader = null;

            try
            {
                connection = Connect();
                var paramsDic = new Dictionary<string, object> { { "@EventsId", id } };
                SqlCommand command = CreateCommandWithStoredProcedure("spGetEventById", connection, paramsDic);
                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    ev = new Event();
                    ev.EventsId = Convert.ToInt32(reader["EventsId"]);
                    ev.StartDateTime = Convert.ToDateTime(reader["StartDateTime"]);
                    ev.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDateTime"]);
                    ev.EventsType = reader["EventsType"].ToString();
                    ev.PhotoUrl = reader["PhotoUrl"].ToString();
                    ev.Description = reader["Description"].ToString();
                    ev.EventsStatus = reader["EventsStatus"].ToString();
                    ev.Municipality = reader["Municipality"].ToString();
                    ev.ResponsibleBody = reader["ResponsibleBody"].ToString();
                    ev.LocationsId = Convert.ToInt32(reader["LocationsId"]);

                    ev.City = reader["City"].ToString();
                    ev.Street = reader["Street"].ToString();
                    ev.HouseNumber = reader["HouseNumber"].ToString();
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (connection != null) connection.Close();
            }
            return ev;
        }

        // ---------------------------------------------------------------------------------
        // יצירת אירוע חדש
        // ---------------------------------------------------------------------------------
        public int CreateEvent(Event ev)
        {
            int newId = -1;
            SqlConnection connection = null;
            SqlDataReader reader = null;

            try
            {
                connection = Connect();
                var p = new Dictionary<string, object>
                {
                    { "@StartDateTime", ev.StartDateTime },
                    { "@EndDateTime", ev.EndDateTime },
                    { "@EventsType", ev.EventsType },
                    { "@PhotoUrl", ev.PhotoUrl },
                    { "@Description", ev.Description },
                    { "@Municipality", ev.Municipality },
                    { "@ResponsibleBody", ev.ResponsibleBody },
                    { "@EventsStatus", ev.EventsStatus },
                    { "@LocationsId", ev.LocationsId }
                };

                SqlCommand command = CreateCommandWithStoredProcedure("spCreateEvent", connection, p);

                // Changed from ExecuteScalar to ExecuteReader
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // שימוש ב-Convert.ToInt32 מבצע המרה אוטומטית, גם אם ה-SQL מחזיר Decimal (מה שקורה עם SCOPE_IDENTITY)
                    // for example, if the database returns a decimal (LIKE:5.0) type for the new ID we can still convert it to int
                    if (reader[0] != DBNull.Value)
                    {
                        newId = Convert.ToInt32(reader[0]);
                    }
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (connection != null) connection.Close();
            }
            return newId;
        }

        // ---------------------------------------------------------------------------------
        // מחיקת אירוע (מוחק גם את הדירוגים הקשורים אליו ב-SP)
        // ---------------------------------------------------------------------------------
        public bool DeleteEvent(int id)
        {
            SqlConnection connection = null;
            try
            {
                connection = Connect();
                var p = new Dictionary<string, object> { { "@EventsId", id } };
                SqlCommand command = CreateCommandWithStoredProcedure("spDeleteEvent", connection, p);

                // ExecuteNonQuery מחזיר את מספר השורות שהושפעו
                return command.ExecuteNonQuery() > 0;
            }
            finally
            {
                if (connection != null) connection.Close();
            }
        }
    }
}