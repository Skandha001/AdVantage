// Models/BookingModel.cs
using System;

namespace AdVantageWebApp.Models
{
    public class BookingModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AdId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsApproved { get; set; }
    }
}