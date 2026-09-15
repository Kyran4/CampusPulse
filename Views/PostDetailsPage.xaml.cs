using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class PostDetailsPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public PostDetailsPage(PostDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Poll while a post is open so other users' comments/reactions show
        // up while you're reading, not just next time you open the post.
        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => _ = (BindingContext as PostDetailsViewModel)?.RefreshAsync(showErrorAlert: false);
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
