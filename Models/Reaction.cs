namespace CampusPulse.Models;

public class Reaction
{
    public int ReactionId { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }

    public string Type { get; set; } // like, love, etc.
}
