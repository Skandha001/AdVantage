using System.ComponentModel.DataAnnotations;

namespace AdAdvanMVC.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int AdID { get; set; }

        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public bool IsApproved { get; set; }
    }
}