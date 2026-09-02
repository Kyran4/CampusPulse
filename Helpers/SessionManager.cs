using CampusPulse.Models;

namespace CampusPulse.Helpers;

public static class SessionManager
{
    public static User CurrentUser { get; set; }

    public static bool IsLoggedIn => CurrentUser != null;

    public static bool IsAdmin =>
        CurrentUser != null && CurrentUser.Role == "Admin";

    public static void Logout()
    {
        CurrentUser = null;
    }
}
