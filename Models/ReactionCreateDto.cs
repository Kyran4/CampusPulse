namespace CampusPulse.Models;

public class ReactionCreateDto
{
    public int PostId { get; set; }
    // No UserId - the API derives the owner from the caller's JWT.
    public string Type { get; set; } = null!; // Like / Helpful / Interested
}
