namespace CampusPulse.Models;

public class UpdateProfileDto
{
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string? ProfileImageUrl { get; set; }
}
