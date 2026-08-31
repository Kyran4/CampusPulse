namespace CampusPulse.Models;

public class Event
{
    public int EventId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public int CategoryId { get; set; }
    public int CreatedBy { get; set; }
    public int Capacity { get; set; }
}
