using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class LocationsDAL : DBServices
    {

        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;

        // הוספת מיקום חדש - קריאה ל-SP: spAddLocation
        // מחזיר את ה-LocationsId החדש
        public int AddLocation(Location location)
        {
            int newId = -1;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@City", location.City);
                paramDic.Add("@Street", location.Street);
                paramDic.Add("@HouseNumber", location.HouseNumber);
                paramDic.Add("@HouseType", location.HouseType);
                paramDic.Add("@Floor", (object?)location.Floor ?? DBNull.Value);

                command = CreateCommandWithStoredProcedure("spAddLocation", connection, paramDic);

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

        // שליפת כל המיקומים - קריאה ל-SP: spGetAllLocations
        public List<Location> GetAllLocations()
        {
            List<Location> locations = new List<Location>();

            try
            {
                connection = Connect();

                // אין פרמטרים
                command = CreateCommandWithStoredProcedure("spGetAllLocations", connection, null);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Location loc = new Location();
                    loc.LocationsId = Convert.ToInt32(reader["LocationsId"]);
                    loc.City = reader["City"].ToString();
                    loc.Street = reader["Street"].ToString();
                    loc.HouseNumber = reader["HouseNumber"].ToString();
                    loc.HouseType = reader["HouseType"].ToString();
                    loc.Floor = reader["Floor"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["Floor"]);

                    locations.Add(loc);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return locations;
        }
    }

}
