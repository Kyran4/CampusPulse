namespace CampusPulse.Models;

public class UserAdminDto
{
    public int UserId { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int PostCount { get; set; }
    public int EventCount { get; set; }
    public int PendingReports { get; set; }
}
