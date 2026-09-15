using CampusPulse.Services;
using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class FeedPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public FeedPage(FeedViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var db = new DatabaseService();
        var token = await db.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        // FeedPage/FeedViewModel are constructed once and reused for the
        // life of the app (Shell doesn't recreate flyout pages on every
        // visit), so the constructor's initial load only ever ran once.
        // Without this, creating a post and navigating back showed the
        // feed exactly as it was before you posted - refreshing here means
        // every time you actually land on this page, it's current.
        if (BindingContext is FeedViewModel vm)
            vm.RefreshCommand.Execute(null);

        // Poll every 7s while this page is actually visible, so posts
        // created by other users show up without a manual refresh. A fresh
        // timer is created each time the page appears and torn down in
        // OnDisappearing - reusing one timer across visits would mean each
        // visit adds another Tick subscriber, firing the refresh multiple
        // times per tick.
        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => _ = (BindingContext as FeedViewModel)?.PollAsync();
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
