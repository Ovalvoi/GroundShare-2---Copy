using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace GroundShare.DAL
{
    // גישה לנתונים עבור דירוגים
    public class RatingsDAL : DBServices
    {
        // ---------------------------------------------------------------------------------
        // הוספת דירוג חדש למסד הנתונים
        // ---------------------------------------------------------------------------------
        public int AddRating(Rating rating)
        {
            int newId = -1;
            SqlConnection connection = null;
            try
            {
                connection = Connect();
                var p = new Dictionary<string, object>
                {
                    { "@UserId", rating.UserId },
                    { "@EventsId", rating.EventsId },
                    { "@OverallScore", rating.OverallScore },
                    { "@NoiseScore", rating.NoiseScore },
                    { "@TrafficScore", rating.TrafficScore },
                    { "@SafetyScore", rating.SafetyScore },
                    { "@Comment", rating.Comment }
                };
                SqlCommand command = CreateCommandWithStoredProcedure("spAddRating", connection, p);
                object scalar = command.ExecuteScalar();
                if (scalar != null) newId = Convert.ToInt32(scalar);
            }
            finally
            {
                if (connection != null) connection.Close();
            }
            return newId;
        }

        // ---------------------------------------------------------------------------------
        // שליפת רשימת דירוגים לפי מזהה אירוע
        // ---------------------------------------------------------------------------------
        public List<Rating> GetRatingsByEvent(int eventId)
        {
            List<Rating> list = new List<Rating>();
            SqlConnection connection = null;
            SqlDataReader reader = null;
            try
            {
                connection = Connect();
                var p = new Dictionary<string, object> { { "@EventsId", eventId } };
                SqlCommand command = CreateCommandWithStoredProcedure("spGetRatingsByEvent", connection, p);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Rating r = new Rating();
                    r.RatingId = Convert.ToInt32(reader["RatingId"]);
                    r.UserId = Convert.ToInt32(reader["UserId"]);
                    r.EventsId = Convert.ToInt32(reader["EventsId"]);
                    // שימוש ב-Convert.ToByte עבור שדות מסוג TinyInt ב-SQL
                    r.OverallScore = Convert.ToByte(reader["OverallScore"]);
                    r.NoiseScore = Convert.ToByte(reader["NoiseScore"]);
                    r.TrafficScore = Convert.ToByte(reader["TrafficScore"]);
                    r.SafetyScore = Convert.ToByte(reader["SafetyScore"]);
                    r.Comment = reader["Comment"].ToString();
                    r.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
                    list.Add(r);
                }
            }
            finally
            {
                if (reader != null) reader.Close();
                if (connection != null) connection.Close();
            }
            return list;
        }
    }
}