using System;
using System.Collections.Generic;
using GroundShare.DAL;

namespace GroundShare.BL
{
    // מחלקה המייצגת מיקום פיזי במערכת (Business Logic Layer)
    // מיקום יכול להיות קשור לאירוע, למשל.
    public class Location
    {
        // ---------------------------------------------------------
        // מאפיינים המייצגים את עמודות טבלת Locations בבסיס הנתונים
        // ---------------------------------------------------------
        public int LocationsId { get; set; } // מזהה ייחודי של המיקום

        // הערה: הוסרו סימני שאלה משדות שהם NOT NULL בבסיס הנתונים כדי לשמור על תאימות
        public string City { get; set; } // עיר
        public string Street { get; set; } // רחוב
        public string HouseNumber { get; set; } // מספר בית
        public string HouseType { get; set; } // סוג המבנה (למשל, "בניין", "בית פרטי")

        public int? Floor { get; set; } // קומה (יכול להיות ריק - Nullable)

        // בנאי ריק - נדרש עבור Deserialization (המרת JSON לאובייקט)
        public Location() { }

        // ---------------------------------------------------------
        // פונקציות - קוראות לשכבת ה-DAL לביצוע פעולות על בסיס הנתונים
        // ---------------------------------------------------------

        // הוספת מיקום חדש (Instance Method)
        // הפונקציה משתמשת בנתונים של האובייקט הנוכחי (this) כדי ליצור רשומה חדשה
        public int Add()
        {
            LocationsDAL dal = new LocationsDAL();
            return dal.AddLocation(this);
        }

        // שליפת כל המיקומים הקיימים (Static Method)
        public static List<Location> GetAll()
        {
            LocationsDAL dal = new LocationsDAL();
            return dal.GetAllLocations();
        }
    }
}