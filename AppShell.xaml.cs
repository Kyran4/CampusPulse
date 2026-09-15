using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Views;
using CampusPulse.Services;

namespace CampusPulse;

public partial class AppShell : Shell
{
    // Routes reachable without being signed in. Everything else is
    // protected by the Navigating guard below - an allow-list rather than a
    // growing block-list, so any new page added later is protected by
    // default instead of accidentally left open.
    private static readonly string[] PublicRoutes = { "LoginPage", "RegisterPage" };

    public AppShell()
    {
        InitializeComponent();

        // Routes for pages that aren't tabs - reached only via GoToAsync,
        // never shown as a persistent nav destination. Tabs themselves
        // (Feed/Events/Create/My Posts/Profile) don't need this - they're
        // already wired up directly in AppShell.xaml.
        Routing.RegisterRoute("EventDetailsPage", typeof(EventDetailsPage));
        Routing.RegisterRoute("PostDetailsPage", typeof(PostDetailsPage));
        Routing.RegisterRoute("CreateEventPage", typeof(CreateEventPage));
        Routing.RegisterRoute("InterestsPage", typeof(InterestsPage));
        Routing.RegisterRoute("AdminHubPage", typeof(AdminHubPage));
        Routing.RegisterRoute("AdminDashboardPage", typeof(AdminDashboardPage));
        Routing.RegisterRoute("AdminModerationPage", typeof(AdminModerationPage));
        Routing.RegisterRoute("ReportsPage", typeof(ReportsPage));
        Routing.RegisterRoute("AdminUsersPage", typeof(AdminUsersPage));

        // Global auth guard - runs before every navigation. Without this, a
        // signed-out user could still reach a protected route directly
        // (e.g. a stale deep link) even with no tab bar pointing at it.
        Navigating += OnShellNavigating;

        // Check auth on startup - deliberately on Loaded, not fired
        // directly here. The constructor runs before the native Shell UI
        // has actually finished initializing, so touching tab visibility
        // this early sometimes doesn't get picked up by the rendered UI
        // until something else forces a refresh.
        Loaded += OnShellLoaded;
    }

    private async void OnShellLoaded(object sender, EventArgs e)
    {
        Loaded -= OnShellLoaded;
        await EnsureAuthenticatedAsync();
    }

    private async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
    {
        var target = e.Target?.Location?.OriginalString ?? string.Empty;

        var isPublic = PublicRoutes.Any(r => target.Contains(r, StringComparison.OrdinalIgnoreCase));
        if (isPublic) return;

        if (!SessionManager.IsLoggedIn)
        {
            e.Cancel();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    public async Task EnsureAuthenticatedAsync()
    {
        var db = new DatabaseService();
        var token = await db.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            RemoveAdminTab();
            SessionManager.Logout();
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // We have a token from a previous session - restore who's logged in
        // and re-show the Admin tab if they're an Admin. Without this, an
        // Admin who restarted the app would keep a valid token but lose
        // their Admin tab until they logged out and back in.
        var user = db.LoadUser<User>();
        SessionManager.CurrentUser = user;

        if (SessionManager.IsAdmin)
            AddAdminTab();
        else
            RemoveAdminTab();

        // Explicit navigation rather than relying on Shell's default
        // initial route: that default can fire before this async method
        // finishes, when SessionManager.IsLoggedIn is still false - the
        // Navigating guard would correctly-but-wrongly bounce it to Login.
        await Shell.Current.GoToAsync("//MainTabs");
    }

    // ---------------------------------------------------------
    // ADMIN TAB (visible only for Admins) - the one piece of the bottom bar
    // that still needs to change based on who's signed in. Everything else
    // (Feed/Events/Create/My Posts/Profile) is a fixed, always-visible tab
    // once you're authenticated at all.
    // ---------------------------------------------------------
    public void AddAdminTab()
    {
        if (MainTabBar.Items.Any(i => i.Route == "AdminHubPage"))
            return;

        var adminTab = new Tab
        {
            Title = "Admin",
            Icon = "admin.png",
            Route = "AdminHubPage",
            Items =
            {
                new ShellContent
                {
                    Route = "AdminHubPage",
                    ContentTemplate = new DataTemplate(typeof(AdminHubPage))
                }
            }
        };

        MainTabBar.Items.Add(adminTab);
    }

    public void RemoveAdminTab()
    {
        var adminTab = MainTabBar.Items.FirstOrDefault(i => i.Route == "AdminHubPage");
        if (adminTab != null)
            MainTabBar.Items.Remove(adminTab);
    }
}
