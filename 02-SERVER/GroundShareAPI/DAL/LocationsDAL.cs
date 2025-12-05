using GroundShare.BL;
using Microsoft.Data.SqlClient;

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
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = Connect();
                var p = new Dictionary<string, object>
                {
                    { "@City", location.City },
                    { "@Street", location.Street },
                    { "@HouseNumber", location.HouseNumber },
                    { "@HouseType", location.HouseType },
                    { "@Floor", location.Floor }
                };
                SqlCommand command = CreateCommandWithStoredProcedure("spAddLocation", connection, p);

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
        // שליפת כל המיקומים
        // ---------------------------------------------------------------------------------
        public List<Location> GetAllLocations()
        {
            List<Location> list = new List<Location>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = Connect();
                SqlCommand command = CreateCommandWithStoredProcedure("spGetAllLocations", connection, null);
                reader = command.ExecuteReader();

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
            finally
            {
                if (reader != null) reader.Close();
                if (connection != null) connection.Close();
            }
            return list;
        }
    }
}