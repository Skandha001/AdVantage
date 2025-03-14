public class Feedback
{
    public DateTime FeedbackDate { get; set; } = DateTime.Now;
    public int FeedBackID { get; set; }
    public int AdID { get; set; }
    public int UserID { get; set; }
    public string Message { get; set; }
}