using System.ComponentModel.DataAnnotations;
public class User
{
    [Key]
    public int UserID { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // Store hashed passwords!

}
