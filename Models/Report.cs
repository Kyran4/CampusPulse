namespace CampusPulse.Models;

public class Report
{
    public int ReportId { get; set; }
    public int ReportedByUserId { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public string Reason { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Reviewed, Action Taken, Dismissed
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? ReviewedBy { get; set; }
}
