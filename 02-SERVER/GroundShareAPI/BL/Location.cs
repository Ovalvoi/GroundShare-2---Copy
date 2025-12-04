using System;
using System.Collections.Generic;
using GroundShare.DAL;

namespace GroundShare.BL
{
    // מחלקה המייצגת מיקום (כתובת)
    public class Location
    {
        public int LocationsId { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? HouseType { get; set; } // בניין / בית פרטי וכו'
        public int? Floor { get; set; }

        public Location() { }

        // הוספת מיקום (Instance Method)
        public int Add()
        {
            LocationsDAL dal = new LocationsDAL();
            return dal.AddLocation(this);
        }

        // שליפת כל המיקומים (Static Method) - לשימוש ב-Dropdown בטופס
        public static List<Location> GetAll()
        {
            LocationsDAL dal = new LocationsDAL();
            return dal.GetAllLocations();
        }
    }
}