using System;
using System.Collections.Generic;
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

        public Location()
        {
        }

        public int Add()
        {
            LocationsDAL dal = new LocationsDAL();
            return dal.AddLocation(this);
        }

        public static List<Location> GetAll()
        {
            LocationsDAL dal = new LocationsDAL();
            return dal.GetAllLocations();
        }
    }
}
