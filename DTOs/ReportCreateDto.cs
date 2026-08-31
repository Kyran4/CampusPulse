namespace CampusPulse.DTOs;

public class ReportCreateDto
{
    public int? PostId { get; set; }
    public int? CommentId { get; set; }
    public string Reason { get; set; }
}
