using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System;

namespace GroundShare.DAL
{
    // גישה לנתונים עבור מיקומים
    public class LocationsDAL : DBServices
    {
        // ---------------------------------------------------------------------------------
        // הוספת מיקום חדש
        // ---------------------------------------------------------------------------------
        public int AddLocation(Location location)
        {
            int newId = -1;

            // יצירת החיבור בתוך בלוק using (נסגר אוטומטית בסיום)
            using (SqlConnection connection = Connect())
            {
                var p = new Dictionary<string, object>
                {
                    { "@City", location.City },
                    { "@Street", location.Street },
                    { "@HouseNumber", location.HouseNumber },
                    { "@HouseType", location.HouseType },
                    { "@Floor", location.Floor }
                };

                // יצירת הפקודה בתוך בלוק using
                using (SqlCommand command = CreateCommandWithStoredProcedure("spAddLocation", connection, p))
                {
                    // ביצוע השאילתה בתוך בלוק using
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // שימוש ב-Convert.ToInt32 מבצע המרה אוטומטית, גם אם ה-SQL מחזיר Decimal (מה שקורה עם SCOPE_IDENTITY)
                            if (reader[0] != DBNull.Value)
                            {
                                newId = Convert.ToInt32(reader[0]);
                            }
                        }
                    }
                }
            }
            return newId;
        }

        // ---------------------------------------------------------------------------------
        // שליפת כל המיקומים
        // ---------------------------------------------------------------------------------
        public List<Location> GetAllLocations()
        {
            List<Location> list = new List<Location>();

            using (SqlConnection connection = Connect())
            {
                using (SqlCommand command = CreateCommandWithStoredProcedure("spGetAllLocations", connection, null))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Location loc = new Location();
                            loc.LocationsId = Convert.ToInt32(reader["LocationsId"]);
                            loc.City = reader["City"].ToString();
                            loc.Street = reader["Street"].ToString();
                            loc.HouseNumber = reader["HouseNumber"].ToString();
                            loc.HouseType = reader["HouseType"].ToString();

                            // המרה בטוחה של שדה שיכול להיות NULL
                            loc.Floor = reader["Floor"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["Floor"]);

                            list.Add(loc);
                        }
                    }
                }
            }
            return list;
        }
    }
}