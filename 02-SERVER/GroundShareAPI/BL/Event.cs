using System;
using System.Collections.Generic;
using GroundShare.DAL;

namespace GroundShare.BL
{
    // מחלקה המייצגת אירוע במערכת (Business Logic Layer)
    public class Event
    {
        // ---------------------------------------------------------
        // מאפיינים המייצגים את עמודות טבלת Events בבסיס הנתונים
        // ---------------------------------------------------------
        public int EventsId { get; set; } // מזהה ייחודי של האירוע
        public DateTime StartDateTime { get; set; } // תאריך ושעת התחלה
        public DateTime? EndDateTime { get; set; } // תאריך ושעת סיום (יכול להיות ריק - Nullable)

        // הערה: הוסרו סימני שאלה משדות שהם NOT NULL בבסיס הנתונים כדי לשמור על תאימות
        public string EventsType { get; set; } // סוג האירוע (למשל, "עבודת תשתית")

        public string? PhotoUrl { get; set; } // נתיב לתמונה (יכול להיות ריק)
        public string? Description { get; set; } // תיאור האירוע (יכול להיות ריק)

        public string Municipality { get; set; } // רשות מוניציפלית אחראית
        public string ResponsibleBody { get; set; } // גוף מבצע
        public string EventsStatus { get; set; } // סטטוס האירוע (קרה, קורה, יקרה)
        public int LocationsId { get; set; } // מזהה של המיקום (מפתח זר לטבלת Locations)

        // ---------------------------------------------------------
        // מאפיינים לתצוגה בלבד - מתקבלים משאילתת JOIN עם טבלאות אחרות
        // ---------------------------------------------------------
        public string? City { get; set; } // שם העיר (מטבלת Locations)
        public string? Street { get; set; } // שם הרחוב (מטבלת Locations)
        public string? HouseNumber { get; set; } // מספר הבית (מטבלת Locations)

        public double AvgRating { get; set; } // דירוג ממוצע (מחושב מטבלת Rating)
        public int RatingCount { get; set; } // מספר המדרגים (מחושב מטבלת Rating)

        // בנאי ריק - נדרש עבור Deserialization (המרת JSON לאובייקט)
        public Event() { }

        // ---------------------------------------------------------
        // פונקציות סטטיות - קוראות לשכבת ה-DAL לביצוע פעולות על בסיס הנתונים
        // ---------------------------------------------------------

        // שליפת כל האירועים
        public static List<Event> GetAll()
        {
            EventsDAL dal = new EventsDAL();
            return dal.GetAllEvents();
        }

        // שליפת אירוע בודד לפי מזהה (ID)
        public static Event GetById(int id)
        {
            EventsDAL dal = new EventsDAL();
            return dal.GetEventById(id);
        }

        // יצירת אירוע חדש (Instance Method)
        // הפונקציה משתמשת בנתונים של האובייקט הנוכחי (this) כדי ליצור רשומה חדשה
        public int Create()
        {
            EventsDAL dal = new EventsDAL();
            int newId = dal.CreateEvent(this); // קריאה ל-DAL עם האובייקט הנוכחי
            this.EventsId = newId; // עדכון המזהה של האובייקט לאחר שנוצר ב-DB
            return newId; // החזרת המזהה החדש
        }

        // ---------------------------------------------------------
        //  פונקציה לעדכון סטטוס (PUT)
        // ---------------------------------------------------------
        public bool UpdateStatus(string newStatus)
        {
            EventsDAL dal = new EventsDAL();
            // אנו מעבירים את ה-ID של המופע הנוכחי ואת הסטטוס החדש
            return dal.UpdateEventStatus(this.EventsId, newStatus);
        }

        // מחיקת אירוע לפי מזהה (ID)
        public static bool Delete(int id)
        {
            EventsDAL dal = new EventsDAL();
            return dal.DeleteEvent(id);
        }

        // ---------------------------------------------------------
        // Ad-Hoc: Get Statistics
        // Calls the specific DAL logic
        // ---------------------------------------------------------
        public static Object GetStats()
        {
            EventsDAL dal = new EventsDAL();
            // Now calling the method on EventsDAL, not DBServices
            return dal.GetEventTypeStats();
        }

        
    }
}