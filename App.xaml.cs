using CampusPulse.Services;

namespace CampusPulse;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    // AppShell's constructor already calls EnsureAuthenticatedAsync(), which
    // checks the token, restores SessionManager.CurrentUser, rebuilds the
    // Admin/Create-Post flyout items, and navigates accordingly. This method
    // used to ALSO check the token and navigate independently - two
    // unsynchronized async flows racing on every launch. Whichever won the
    // race decided where you landed; if App.OnStart's bare navigation to
    // FeedPage won before AppShell finished restoring the session, you'd be
    // on the Feed page with SessionManager.CurrentUser still null and the
    // Create Post flyout item not yet added. Removing the duplicate here
    // makes AppShell the single source of truth for startup navigation.
}
