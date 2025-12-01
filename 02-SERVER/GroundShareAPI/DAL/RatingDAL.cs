using GroundShare.BL;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.DAL
{
    public class RatingsDAL : DBServices
    {
        private SqlDataReader reader;
        private SqlConnection connection;
        private SqlCommand command;

        // הוספת דירוג חדש - SP: spAddRating
        // מחזיר RatingId חדש
        public int AddRating(Rating rating)
        {
            int newId = -1;

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@UserId", rating.UserId);
                paramDic.Add("@EventsId", rating.EventsId);
                paramDic.Add("@OverallScore", rating.OverallScore);
                paramDic.Add("@NoiseScore", rating.NoiseScore);
                paramDic.Add("@TrafficScore", rating.TrafficScore);
                paramDic.Add("@SafetyScore", rating.SafetyScore);
                paramDic.Add("@Comment", (object?)rating.Comment ?? DBNull.Value);

                command = CreateCommandWithStoredProcedure("spAddRating", connection, paramDic);

                object scalar = command.ExecuteScalar();

                if (scalar != null && scalar != DBNull.Value)
                {
                    if (scalar is decimal dec)
                        newId = (int)dec;
                    else
                        newId = Convert.ToInt32(scalar);
                }
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return newId;
        }

        // שליפת כל הדירוגים של אירוע - SP: spGetRatingsByEvent
        public List<Rating> GetRatingsByEvent(int eventId)
        {
            List<Rating> ratings = new List<Rating>();

            try
            {
                connection = Connect();

                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("@EventsId", eventId);

                command = CreateCommandWithStoredProcedure("spGetRatingsByEvent", connection, paramDic);

                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Rating r = new Rating();

                    r.RatingId = Convert.ToInt32(reader["RatingId"]);
                    r.UserId = Convert.ToInt32(reader["UserId"]);
                    r.EventsId = Convert.ToInt32(reader["EventsId"]);
                    r.OverallScore = Convert.ToByte(reader["OverallScore"]);
                    r.NoiseScore = Convert.ToByte(reader["NoiseScore"]);
                    r.TrafficScore = Convert.ToByte(reader["TrafficScore"]);
                    r.SafetyScore = Convert.ToByte(reader["SafetyScore"]);
                    r.Comment = reader["Comment"].ToString();
                    r.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);

                    ratings.Add(r);
                }
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();

                if (connection != null && connection.State != ConnectionState.Closed)
                    connection.Close();
            }

            return ratings;
        }
    }


}
