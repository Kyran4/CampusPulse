namespace CampusPulse.Models;

public class User
{
    public int UserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // "Student" or "Admin"
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
