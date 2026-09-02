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

        // Database
        builder.Services.AddSingleton<DatabaseService>();

        // Services
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<PostService>();
        builder.Services.AddSingleton<CommentService>();
        builder.Services.AddSingleton<ReactionService>();
        builder.Services.AddSingleton<EventService>();
        builder.Services.AddSingleton<ReportService>();
        builder.Services.AddSingleton<CategoryService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<FeedViewModel>();
        builder.Services.AddTransient<CreatePostViewModel>();
        builder.Services.AddTransient<PostDetailsViewModel>();
        builder.Services.AddTransient<EventsViewModel>();
        builder.Services.AddTransient<EventDetailsViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<MyPostsViewModel>();
        builder.Services.AddTransient<ReportsViewModel>();
        builder.Services.AddTransient<AdminDashboardViewModel>();
        builder.Services.AddTransient<AdminModerationViewModel>();

        // Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<FeedPage>();
        builder.Services.AddTransient<CreatePostPage>();
        builder.Services.AddTransient<PostDetailsPage>();
        builder.Services.AddTransient<EventsPage>();
        builder.Services.AddTransient<EventDetailsPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<MyPostsPage>();
        builder.Services.AddTransient<ReportsPage>();
        builder.Services.AddTransient<AdminDashboardPage>();
        builder.Services.AddTransient<AdminModerationPage>();
        builder.Services.AddTransient<AboutPage>();

        return builder.Build();
    }
}
