namespace CampusPulse.Models;

public class PostCreateDto
{
    // No UserId - the API derives the owner from the caller's JWT.
    public int CategoryId { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }

    public string? ImageBase64 { get; set; }
}
