using System;
using System.Collections.Generic;
using System.Data;
using GroundShare.DAL;

namespace GroundShare.BL
{
    public class Location
    {
        public int LocationsId { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string HouseType { get; set; }
        public int? Floor { get; set; }

        public Location() { }

        // Insert
        public int Insert()
        {
            LocationsDAL dal = new LocationsDAL();
            return dal.InsertLocation(this.City, this.Street, this.HouseNumber, this.HouseType, this.Floor);
        }

        // Read
        public List<Location> Read()
        {
            LocationsDAL dal = new LocationsDAL();
            List<Location> list = new List<Location>();

            DataTable dt = dal.GetAllLocations();

            foreach (DataRow dr in dt.Rows)
            {
                Location loc = new Location();
                loc.LocationsId = Convert.ToInt32(dr["LocationsId"]);
                loc.City = dr["City"].ToString();
                loc.Street = dr["Street"].ToString();
                loc.HouseNumber = dr["HouseNumber"].ToString();
                loc.HouseType = dr["HouseType"].ToString();
                if (dr["Floor"] != DBNull.Value) loc.Floor = Convert.ToInt32(dr["Floor"]);

                list.Add(loc);
            }
            return list;
        }
    }
}