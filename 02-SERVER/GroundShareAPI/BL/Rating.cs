using GroundShare.DAL;
using System;
using System.Collections.Generic;

namespace GroundShare.BL
{
    // מחלקה המייצגת דירוג (ביקורת) של משתמש על אירוע (Business Logic Layer)
    public class Rating
    {
        // ---------------------------------------------------------
        // מאפיינים המייצגים את עמודות טבלת Rating בבסיס הנתונים
        // ---------------------------------------------------------
        public int RatingId { get; set; } // מזהה ייחודי של הדירוג
        public int UserId { get; set; } // מזהה המשתמש שדירג (מפתח זר לטבלת Users)
        public int EventsId { get; set; } // מזהה האירוע המדורג (מפתח זר לטבלת Events)
        public int OverallScore { get; set; } // ציון כללי
        public int NoiseScore { get; set; }   // ציון רעש
        public int TrafficScore { get; set; } // ציון תנועה
        public int SafetyScore { get; set; }  // ציון בטיחות
        public string? Comment { get; set; } // הערה מילולית (יכול להיות ריק)
        public DateTime CreatedAt { get; set; } // תאריך ושעת יצירת הדירוג

        // בנאי ריק - נדרש עבור Deserialization (המרת JSON לאובייקט)
        public Rating() { }

        // ---------------------------------------------------------
        // פונקציות - קוראות לשכבת ה-DAL לביצוע פעולות על בסיס הנתונים
        // ---------------------------------------------------------

        // הוספת דירוג (Instance Method)
        // הפונקציה משתמשת בנתונים של האובייקט הנוכחי (this) כדי ליצור רשומת דירוג חדשה
        public int Add()
        {
            RatingsDAL dal = new RatingsDAL();
            return dal.AddRating(this);
        }

        // שליפת דירוגים לפי אירוע (Static Method)
        // הפונקציה מקבלת מזהה אירוע ומחזירה את כל הדירוגים המשויכים אליו
        public static List<Rating> GetByEvent(int eventId)
        {
            RatingsDAL dal = new RatingsDAL();
            return dal.GetRatingsByEvent(eventId);
        }
    }
}