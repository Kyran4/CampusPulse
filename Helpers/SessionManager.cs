using CampusPulse.Models;

namespace CampusPulse.Helpers;

public static class SessionManager
{
    public static User? CurrentUser { get; set; }

    public static bool IsLoggedIn => CurrentUser != null;

    public static bool IsAdmin =>
        CurrentUser != null &&
        string.Equals(CurrentUser.Role, "Admin", StringComparison.OrdinalIgnoreCase);

    public static void Logout()
    {
        CurrentUser = null;
    }
}