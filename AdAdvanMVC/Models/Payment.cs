using System.ComponentModel.DataAnnotations;

namespace AdAdvanMVC.Models
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        public int BookingId { get; set; }

        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }
        public int Amount { get; set; }
        public string PaymentMethod { get; set; } = "Cash"; // e.g., "Credit Card", "Net Banking", "UPI"
        public bool IsSuccessful { get; set; }
    }
}
