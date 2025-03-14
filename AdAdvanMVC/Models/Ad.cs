using System.ComponentModel.DataAnnotations;

namespace AdAdvanMVC.Models
{
    public class Ad
    {
        [Key]
        public int AdID { get; set; }
        public string AdType { get; set; } // "Digital", "Newspaper", "Hoarding"
        public string Description { get; set; }
        public int PricePerDayOrOneIssue { get; set; }
        public string PriceUnit { get; set; }
    }
}