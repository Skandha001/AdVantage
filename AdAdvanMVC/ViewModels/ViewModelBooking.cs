using System.ComponentModel.DataAnnotations;

namespace AdAdvanMVC.Models
{
    public class ViewModelBooking
    {
        [Key]
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}