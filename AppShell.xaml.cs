using CampusPulse.Helpers;

namespace CampusPulse;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        ApplyRoleVisibility();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ApplyRoleVisibility();
    }

    private void ApplyRoleVisibility()
    {
        bool isAdmin = SessionManager.IsAdmin;

        // Admin pages
        var adminDashboard = this.Items.FirstOrDefault(i => i.Route == "AdminDashboardPage");
        var adminModeration = this.Items.FirstOrDefault(i => i.Route == "AdminModerationPage");
        var reports = this.Items.FirstOrDefault(i => i.Route == "ReportsPage");

        if (adminDashboard != null) adminDashboard.IsVisible = isAdmin;
        if (adminModeration != null) adminModeration.IsVisible = isAdmin;
        if (reports != null) reports.IsVisible = isAdmin;
    }
}
