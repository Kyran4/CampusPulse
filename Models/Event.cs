namespace CampusPulse.Models;

public class Event
{
    public int EventId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string? ImageBase64 { get; set; }

    public int CategoryId { get; set; }
    public int? Capacity { get; set; }
    public int CreatedByUserId { get; set; }
    public bool IsCancelled { get; set; }

    public Category Category { get; set; }
    public List<EventRegistration> Registrations { get; set; } = new();
}
