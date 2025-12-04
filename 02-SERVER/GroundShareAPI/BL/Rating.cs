using GroundShare.DAL;
using System;
using System.Collections.Generic;

namespace GroundShare.BL
{
    // מחלקה המייצגת דירוג (ביקורת) של משתמש על אירוע
    public class Rating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int EventsId { get; set; }
        public int OverallScore { get; set; } // ציון כללי
        public int NoiseScore { get; set; }   // ציון רעש
        public int TrafficScore { get; set; } // ציון תנועה
        public int SafetyScore { get; set; }  // ציון בטיחות
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Rating() { }

        // הוספת דירוג (Instance Method)
        public int Add()
        {
            RatingsDAL dal = new RatingsDAL();
            return dal.AddRating(this);
        }

        // שליפת דירוגים לפי אירוע (Static Method)
        public static List<Rating> GetByEvent(int eventId)
        {
            RatingsDAL dal = new RatingsDAL();
            return dal.GetRatingsByEvent(eventId);
        }
    }
}