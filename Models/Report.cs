namespace CampusPulse.Models;

public class Report
{
    public int ReportId { get; set; }
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public int UserId { get; set; }

    public string Reason { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = "Pending";
    public int? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }

    public Post Post { get; set; }
    public Comment Comment { get; set; }
    public User User { get; set; }
    public User ReviewedBy { get; set; }
}
