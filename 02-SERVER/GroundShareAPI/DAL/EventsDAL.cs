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

            // Fix: Using 'using' block for automatic resource management
            using (SqlConnection connection = Connect())
            {
                SqlCommand command = CreateCommandWithStoredProcedure("spGetAllEvents", connection, null);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Event ev = new Event();
                        ev.EventsId = Convert.ToInt32(reader["EventsId"]);
                        ev.StartDateTime = Convert.ToDateTime(reader["StartDateTime"]);
                        ev.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["EndDateTime"]);
                        ev.EventsType = reader["EventsType"].ToString();
                        ev.PhotoUrl = reader["PhotoUrl"].ToString();
                        ev.Description = reader["Description"].ToString();
                        ev.EventsStatus = reader["EventsStatus"].ToString();
                        ev.Municipality = reader["Municipality"].ToString();
                        ev.ResponsibleBody = reader["ResponsibleBody"].ToString();

                        ev.City = reader["City"].ToString();
                        ev.Street = reader["Street"].ToString();
                        ev.HouseNumber = reader["HouseNumber"].ToString();

                        ev.AvgRating = Convert.ToDouble(reader["AvgRating"]);
                        ev.RatingCount = Convert.ToInt32(reader["RatingCount"]);

                        list.Add(ev);
                    }
                }
            }
            return list;
        }

        // ---------------------------------------------------------------------------------
        // שליפת אירוע בודד לפי מזהה spGetEventById
        // ---------------------------------------------------------------------------------
        public Event GetEventById(int id)
        {
            Event ev = null;

            using (SqlConnection connection = Connect())
            {
                var paramsDic = new Dictionary<string, object> { { "@EventsId", id } };
                SqlCommand command = CreateCommandWithStoredProcedure("spGetEventById", connection, paramsDic);

                using (SqlDataReader reader = command.ExecuteReader())
                {
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
            }
            return ev;
        }

        // ---------------------------------------------------------------------------------
        // יצירת אירוע חדש spCreateEvent
        // ---------------------------------------------------------------------------------
        public int CreateEvent(Event ev)
        {
            int newId = -1;

            using (SqlConnection connection = Connect())
            {
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

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader[0] != DBNull.Value)
                        {
                            newId = Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
            return newId;
        }

        // ---------------------------------------------------------------------------------
        // עדכון סטטוס אירוע PUT spUpdateEventStatus
        // ---------------------------------------------------------------------------------
        public bool UpdateEventStatus(int id, string status)
        {
            using (SqlConnection connection = Connect())
            {
                var p = new Dictionary<string, object>
        {
            { "@EventsId", id },
            { "@NewStatus", status }
        };

                SqlCommand command = CreateCommandWithStoredProcedure("spUpdateEventStatus", connection, p);
                return command.ExecuteNonQuery() > 0;
            }
        }

        // ---------------------------------------------------------------------------------
        // מחיקת אירוע (מוחק גם את הדירוגים הקשורים אליו ב-SP) spDeleteEvent
        // ---------------------------------------------------------------------------------
        public bool DeleteEvent(int id)
        {
            using (SqlConnection connection = Connect())
            {
                var p = new Dictionary<string, object> { { "@EventsId", id } };
                SqlCommand command = CreateCommandWithStoredProcedure("spDeleteEvent", connection, p);

                return command.ExecuteNonQuery() > 0;
            }
        }

        // ---------------------------------------------------------------------------------
        // AD-HOC METHOD (Implementation)
        // Returns dynamic statistics about event types
        // ---------------------------------------------------------------------------------
        public Object GetEventTypeStats()
        {
            // Using a list of anonymous objects to hold the results
            List<Object> listObjs = new List<Object>();

            using (SqlConnection connection = Connect())
            {
                // Using the SP we created
                SqlCommand command = CreateCommandWithStoredProcedure("spGetEventTypeStats", connection, null);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create an anonymous object dynamically
                        listObjs.Add(new
                        {
                            type = reader["EventsType"].ToString(),
                            amount = Convert.ToInt32(reader["Total"])
                        });
                    }
                }
            }
            return listObjs;
        }
    }
}