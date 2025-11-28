using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class LocationsDAL
    {
        private DBServices _db;

        public LocationsDAL()
        {
            _db = new DBServices();
        }

        public int InsertLocation(string city, string street, string houseNum, string type, int? floor)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@City", city },
                { "@Street", street },
                { "@HouseNumber", houseNum },
                { "@HouseType", type },
                { "@Floor", floor }
            };

            return _db.ExecuteNonQuery("spAddLocation", parameters);
        }

        public DataTable GetAllLocations()
        {
            return _db.ExecuteReader("spGetAllLocations", null);
        }
    }
}