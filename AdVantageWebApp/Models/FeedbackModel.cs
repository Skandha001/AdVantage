// Models/FeedbackModel.cs
using System;

namespace AdVantageWebApp.Models
{
    public class FeedbackModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public DateTime FeedbackDate { get; set; }
        public int AdId { get; set; }
    }
}