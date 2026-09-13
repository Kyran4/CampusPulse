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
    private static readonly string[] PublicRoutes =
    {
        "LoginPage", "LoginPageFlyout", "RegisterPage", "RegisterPageFlyout"
    };

    public AppShell()
    {
        InitializeComponent();

        // Existing route registrations
        Routing.RegisterRoute("CreatePostPage", typeof(CreatePostPage));
        Routing.RegisterRoute("EventDetailsPage", typeof(EventDetailsPage));
        Routing.RegisterRoute("PostDetailsPage", typeof(PostDetailsPage));
        Routing.RegisterRoute("RegisterPage", typeof(RegisterPage));
        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("CreateEventPage", typeof(CreateEventPage));

        // Global auth guard - runs before every navigation, flyout tap
        // included. Without this, a signed-out user could still reach
        // Feed/Events/Profile/etc directly (e.g. by tapping a flyout item
        // that's technically still registered) even though the menu tries
        // to hide them.
        Navigating += OnShellNavigating;

        // Check auth on startup
        _ = EnsureAuthenticatedAsync();
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
            // Remove admin/member-only pages if any were added
            RemoveAdminPages();
            RemoveCreatePostFlyout();
            RemoveMemberPages();
            SessionManager.Logout();
            AddAuthFlyout();

            // Redirect to login
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // We have a token from a previous session - restore who's logged in
        // and re-show the Admin flyout items if they're an Admin. Without
        // this, an Admin who restarted the app would keep a valid token but
        // lose their Admin menu until they logged out and back in.
        var user = db.LoadUser<User>();
        SessionManager.CurrentUser = user;

        RemoveAuthFlyout();
        AddCreatePostFlyout();
        AddMemberPages();

        if (SessionManager.IsAdmin)
            AddAdminPages();
        else
            RemoveAdminPages();

        // Explicit navigation rather than relying on whatever Shell's
        // default initial route happened to be: that default navigation
        // can fire before this async method finishes, when
        // SessionManager.IsLoggedIn is still false - the Navigating guard
        // would correctly-but-wrongly bounce it to Login. This makes the
        // authenticated case deterministic regardless of that race.
        await Shell.Current.GoToAsync("//FeedPage");
    }

    // ---------------------------------------------------------
    // LOGIN / REGISTER FLYOUT (visible only when logged out)
    // ---------------------------------------------------------
    // Login/Register are declared in AppShell.xaml with
    // FlyoutItemIsVisible="False" so they're never implicitly shown - the
    // actual visible menu entries are these dynamically added ones,
    // following the same pattern as AddCreatePostFlyout/AddAdminPages.
    public void AddAuthFlyout()
    {
        if (Items.Any(i => i.Route == "LoginPageFlyout" || i.Route == "RegisterPageFlyout"))
            return;

        var login = new FlyoutItem
        {
            Title = "Sign In",
            Route = "LoginPageFlyout",
            Items =
            {
                new ShellContent
                {
                    Route = "LoginPageFlyout",
                    ContentTemplate = new DataTemplate(typeof(LoginPage))
                }
            }
        };

        var register = new FlyoutItem
        {
            Title = "Register",
            Route = "RegisterPageFlyout",
            Items =
            {
                new ShellContent
                {
                    Route = "RegisterPageFlyout",
                    ContentTemplate = new DataTemplate(typeof(RegisterPage))
                }
            }
        };

        Items.Add(login);
        Items.Add(register);
    }

    public void RemoveAuthFlyout()
    {
        var authItems = Items
            .Where(i => i.Route == "LoginPageFlyout" || i.Route == "RegisterPageFlyout")
            .ToList();

        foreach (var item in authItems)
            Items.Remove(item);
    }

    // ---------------------------------------------------------
    // MEMBER-ONLY PAGES (My Posts / My Interests) - visible only when
    // signed in, same reasoning as Create Post/Admin pages. The static
    // versions in AppShell.xaml are FlyoutItemIsVisible="False"; these
    // dynamic ones (same Route values) are the actual visible entries.
    // ---------------------------------------------------------
    public void AddMemberPages()
    {
        if (Items.Any(i => i.Route == "MyPostsPage" || i.Route == "InterestsPage"))
            return;

        var myPosts = new FlyoutItem
        {
            Title = "My Posts",
            Icon = "posts.png",
            Route = "MyPostsPage",
            Items =
            {
                new ShellContent
                {
                    Route = "MyPostsPage",
                    ContentTemplate = new DataTemplate(typeof(MyPostsPage))
                }
            }
        };

        var interests = new FlyoutItem
        {
            Title = "My Interests",
            Route = "InterestsPage",
            Items =
            {
                new ShellContent
                {
                    Route = "InterestsPage",
                    ContentTemplate = new DataTemplate(typeof(InterestsPage))
                }
            }
        };

        Items.Add(myPosts);
        Items.Add(interests);
    }

    public void RemoveMemberPages()
    {
        var memberItems = Items
            .Where(i => i.Route == "MyPostsPage" || i.Route == "InterestsPage")
            .ToList();

        foreach (var item in memberItems)
            Items.Remove(item);
    }

    // ---------------------------------------------------------
    // ADD ADMIN PAGES
    // ---------------------------------------------------------
    public void AddAdminPages()
    {
        // Prevent duplicates
        if (Items.Any(i =>
            i.Route == "AdminDashboardPage" ||
            i.Route == "AdminModerationPage" ||
            i.Route == "ReportsPage" ||
            i.Route == "AdminUsersPage"))
        {
            return;
        }

        var adminDashboard = new FlyoutItem
        {
            Title = "Admin Dashboard",
            Icon = "admin.png",
            Route = "AdminDashboardPage",
            Items =
            {
                new ShellContent
                {
                    Route = "AdminDashboardPage",
                    ContentTemplate = new DataTemplate(typeof(AdminDashboardPage))
                }
            }
        };

        var moderation = new FlyoutItem
        {
            Title = "Moderation",
            Icon = "moderation.png",
            Route = "AdminModerationPage",
            Items =
            {
                new ShellContent
                {
                    Route = "AdminModerationPage",
                    ContentTemplate = new DataTemplate(typeof(AdminModerationPage))
                }
            }
        };

        var reports = new FlyoutItem
        {
            Title = "Reports",
            Icon = "reports.png",
            Route = "ReportsPage",
            Items =
            {
                new ShellContent
                {
                    Route = "ReportsPage",
                    ContentTemplate = new DataTemplate(typeof(ReportsPage))
                }
            }
        };

        var users = new FlyoutItem
        {
            Title = "Manage Users",
            Icon = "users.png",
            Route = "AdminUsersPage",
            Items =
            {
                new ShellContent
                {
                    Route = "AdminUsersPage",
                    ContentTemplate = new DataTemplate(typeof(AdminUsersPage))
                }
            }
        };

        Items.Add(adminDashboard);
        Items.Add(moderation);
        Items.Add(reports);
        Items.Add(users);
    }

    // ---------------------------------------------------------
    // REMOVE ADMIN PAGES (automatic on logout or non-admin login)
    // ---------------------------------------------------------
    public void RemoveAdminPages()
    {
        var adminItems = Items
            .Where(i =>
                i.Route == "AdminDashboardPage" ||
                i.Route == "AdminModerationPage" ||
                i.Route == "ReportsPage" ||
                i.Route == "AdminUsersPage")
            .ToList();

        foreach (var item in adminItems)
            Items.Remove(item);
    }

    public void AddCreatePostFlyout()
    {
        if (Items.Any(i => i.Route == "CreatePostPage"))
            return;

        var createPost = new FlyoutItem
        {
            Title = "Create Post",
            Icon = "posts.png",
            Route = "CreatePostPage",
            Items =
        {
            new ShellContent
            {
                Route = "CreatePostPage",
                ContentTemplate = new DataTemplate(typeof(CreatePostPage))
            }
        }
        };

        Items.Add(createPost);
    }

    public void RemoveCreatePostFlyout()
    {
        var item = Items.FirstOrDefault(i => i.Route == "CreatePostPage");
        if (item != null)
            Items.Remove(item);
    }
}
