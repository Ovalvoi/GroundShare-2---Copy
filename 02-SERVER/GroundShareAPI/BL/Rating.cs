using GroundShare.DAL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;

namespace GroundShare.BL
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int EventsId { get; set; }
        public int OverallScore { get; set; }
        public int NoiseScore { get; set; }
        public int TrafficScore { get; set; }
        public int SafetyScore { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Rating() { }

    }
}