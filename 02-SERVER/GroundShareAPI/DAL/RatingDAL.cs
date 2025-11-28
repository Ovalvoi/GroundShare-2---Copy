using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class RatingsDAL
    {
        private DBServices _db;

        public RatingsDAL()
        {
            _db = new DBServices();
        }

        public int InsertRating(int userId, int eventId, int overall, int noise, int traffic, int safety, string comment)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId },
                { "@EventsId", eventId },
                { "@OverallScore", overall },
                { "@NoiseScore", noise },
                { "@TrafficScore", traffic },
                { "@SafetyScore", safety },
                { "@Comment", comment }
            };

            return _db.ExecuteNonQuery("spAddRating", parameters);
        }

        // Optional: Get ratings for a specific event
        public DataTable GetRatingsByEvent(int eventId)
        {
            // You would need an spGetRatingsByEvent procedure for this
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@EventsId", eventId }
            };
            return _db.ExecuteReader("spGetRatingsByEvent", parameters);
        }

    }
}