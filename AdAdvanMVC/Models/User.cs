using System.ComponentModel.DataAnnotations;

namespace AdAdvanMVC.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; } = 0;

        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } 

    }
}