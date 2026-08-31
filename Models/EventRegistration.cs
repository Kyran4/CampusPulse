namespace CampusPulse.Models;

public class EventRegistration
{
    public int RegistrationId { get; set; }
    public int EventId { get; set; }
    public int UserId { get; set; }
    public DateTime RegisteredDate { get; set; } = DateTime.UtcNow;
}
