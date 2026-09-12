using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Views;
using CampusPulse.Services;

namespace CampusPulse;

public partial class AppShell : Shell
{
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
        // Check auth on startup
        _ = EnsureAuthenticatedAsync();
    }


    public async Task EnsureAuthenticatedAsync()
    {
        var db = new DatabaseService();
        var token = await db.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            // Remove admin pages if any were added
            RemoveAdminPages();
            RemoveCreatePostFlyout();
            SessionManager.Logout();

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

        AddCreatePostFlyout();

        if (SessionManager.IsAdmin)
            AddAdminPages();
        else
            RemoveAdminPages();
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

    public void AddAuthFlyout()
    {
        // Prevent duplicates
        if (Items.Any(i =>
            i.Route == "LoginPage" ||
            i.Route == "RegisterPage"))
        {
            return;
        }

        var login = new FlyoutItem
        {
            Title = "Login",
            Icon = "login.png",
            Route = "LoginPage",
            Items =
            {
                new ShellContent
                {
                    Route = "LoginPage",
                    ContentTemplate = new DataTemplate(typeof(LoginPage))
                }
            }
        };

        var register = new FlyoutItem
        {
            Title = "Register",
            Icon = "register.png",
            Route = "RegisterPage",
            Items =
            {
                new ShellContent
                {
                    Route = "RegisterPage",
                    ContentTemplate = new DataTemplate(typeof(RegisterPage))
                }
            }
        };

        Items.Add(login);
        Items.Add(register);
    }

    public void RemoveAuthFlyout()
    {
        // Remove authentication-related flyout items (e.g., Login/Register links in unauthenticated state)
        var authItems = Items
            .Where(i =>
                i.Route == "LoginPage" ||
                i.Route == "RegisterPage")
            .ToList();

        foreach (var item in authItems)
            Items.Remove(item);
    }
}
