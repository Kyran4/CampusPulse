namespace CampusPulse.Models;

public class ReportCreateDto
{
    // Exactly one of these should be set.
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    // No UserId - the API derives the reporter from the caller's JWT.

    public string Reason { get; set; }
    public string? Description { get; set; }
}

public class ReportReviewDto
{
    public string Status { get; set; } // Reviewed / ActionTaken / Dismissed
}
