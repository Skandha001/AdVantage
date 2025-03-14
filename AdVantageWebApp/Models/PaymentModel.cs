//Models/PaymentModel.cs
using System;
namespace AdVantageWebApp.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public bool IsSuccessful { get; set; }
    }
}