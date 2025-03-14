using System.ComponentModel.DataAnnotations;

namespace Trial3.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public int BookingId { get; set; }
        public DateTime PaymentDate { get; set; }
        public int Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // e.g., "Credit Card", "Net Banking", "UPI"
        public bool IsSuccessful { get; set; }
    }
}
