namespace CampusPulse.Models;

public class User
{
    public int UserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }

    public string Role { get; set; } // Student or Admin
    public bool IsActive { get; set; } = true;
    public string? ProfileImageUrl { get; set; }

    // No PasswordHash - the API no longer sends it (see UserDto on the API
    // side), and the client should never need it.
}
