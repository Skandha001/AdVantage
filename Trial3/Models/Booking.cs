using System.ComponentModel.DataAnnotations;

public class Booking
{
    [Key]
    public int BookingID { get; set; }
    public int UserID { get; set; }
    public int AdID { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsApproved { get; set; }
}