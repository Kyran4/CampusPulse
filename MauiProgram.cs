using CampusPulse.Helpers;
using CampusPulse.Services;
using CampusPulse.ViewModels;
using CampusPulse.Views;

namespace CampusPulse;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ============================
        //       CORE SERVICES
        // ============================
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<NavigationService>();
        builder.Services.AddSingleton<DialogService>();

        // ============================
        //       API CLIENT
        // ============================
        // ApiAuthHandler reads the saved JWT and attaches it as a Bearer
        // token to every outgoing request. Previously this handler existed
        // but was never wired to any HttpClient, so the token saved at
        // login was never actually sent - every API call went out
        // unauthenticated. Registering it here and attaching it to the
        // client ApiClient uses is what makes [Authorize] on the API
        // actually take effect for the app.
        builder.Services.AddTransient<ApiAuthHandler>();

        builder.Services
            .AddHttpClient<ApiClient>(client =>
            {
                // Default HttpClient timeout is 100 seconds - if a device
                // can't actually reach the API (wrong ServerIp, not on the
                // same network, firewall blocking it), a request just hangs
                // silently for up to that long with no visible feedback,
                // which looks exactly like "nothing happens" rather than an
                // obvious, fast failure. 15s is still generous for a normal
                // request but means a connection problem surfaces quickly.
                client.Timeout = TimeSpan.FromSeconds(15);
            })
            .AddHttpMessageHandler<ApiAuthHandler>();

        // ============================
        //       API SERVICES
        // ============================
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<PostService>();
        builder.Services.AddSingleton<CommentService>();
        builder.Services.AddSingleton<ReactionService>();
        builder.Services.AddSingleton<EventService>();
        builder.Services.AddSingleton<ReportService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<CategoryService>();
        builder.Services.AddSingleton<AdminService>();
        builder.Services.AddSingleton<FollowService>();


        // ============================
        //       VIEWMODELS + PAGES
        // ============================

        // Feed
        builder.Services.AddTransient<FeedViewModel>();
        builder.Services.AddTransient<FeedPage>();

        // My Posts
        builder.Services.AddTransient<MyPostsViewModel>();
        builder.Services.AddTransient<MyPostsPage>();

        // Create Post
        builder.Services.AddTransient<CreatePostViewModel>();
        builder.Services.AddTransient<CreatePostPage>();

        // Post Details
        builder.Services.AddTransient<PostDetailsViewModel>();
        builder.Services.AddTransient<PostDetailsPage>();

        // Events
        builder.Services.AddTransient<EventsViewModel>();
        builder.Services.AddTransient<EventsPage>();

        // Create Event (Admin)
        builder.Services.AddTransient<CreateEventViewModel>();
        builder.Services.AddTransient<CreateEventPage>();

        // Event Details
        builder.Services.AddTransient<EventDetailsViewModel>();
        builder.Services.AddTransient<EventDetailsPage>();

        // Reports
        builder.Services.AddTransient<ReportsViewModel>();
        builder.Services.AddTransient<ReportsPage>();

        // Admin Dashboard
        builder.Services.AddTransient<AdminDashboardViewModel>();
        builder.Services.AddTransient<AdminDashboardPage>();

        // Admin Moderation
        builder.Services.AddTransient<AdminModerationViewModel>();
        builder.Services.AddTransient<AdminModerationPage>();

        // Admin Users
        builder.Services.AddTransient<AdminUsersViewModel>();
        builder.Services.AddTransient<AdminUsersPage>();

        // Interests (follow/unfollow categories)
        builder.Services.AddTransient<InterestsViewModel>();
        builder.Services.AddTransient<InterestsPage>();

        // Profile
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();

        // Authentication
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();

        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();


        return builder.Build();
    }
}
