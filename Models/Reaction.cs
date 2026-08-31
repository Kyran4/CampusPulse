namespace CampusPulse.Models;

public class Reaction
{
    public int ReactionId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string ReactionType { get; set; } // Like, Helpful, Interested
}
