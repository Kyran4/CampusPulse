namespace CampusPulse.Models;

public class EventRegistration
{
    public int EventRegistrationId { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; }
}
