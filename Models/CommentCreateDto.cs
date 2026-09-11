namespace CampusPulse.Models;

public class CommentCreateDto
{
    public int PostId { get; set; }
    // No UserId - the API derives the owner from the caller's JWT.
    public string Content { get; set; }
}

public class CommentUpdateDto
{
    public string Content { get; set; }
}
