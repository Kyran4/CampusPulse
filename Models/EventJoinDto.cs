namespace CampusPulse.Models;

public class EventJoinDto
{
    public int EventId { get; set; }
    // No UserId - the API derives the caller from the JWT.
}
