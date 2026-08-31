namespace CampusPulse.Models;

public class Post
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public bool IsHidden { get; set; } = false;
}
