namespace CampusPulse.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool IsHidden { get; set; }
    public int? ModeratedByUserId { get; set; }
    public string? ModerationReason { get; set; }

    public User User { get; set; }
}
