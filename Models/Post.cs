namespace CampusPulse.Models;

public class Post
{
    public int PostId { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
    public string? ImageBase64 { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool IsHidden { get; set; }

    public User User { get; set; }
    public Category Category { get; set; }
}
