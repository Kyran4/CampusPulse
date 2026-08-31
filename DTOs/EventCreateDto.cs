namespace CampusPulse.DTOs;

public class EventCreateDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public int CategoryId { get; set; }
    public int Capacity { get; set; }
}
