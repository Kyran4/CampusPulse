using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminDashboardPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public AdminDashboardPage(AdminDashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminDashboardViewModel vm)
            vm.RefreshCommand.Execute(null);

        // So stats (post/user counts, pending reports) stay current while
        // an admin is looking at the dashboard.
        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => _ = (BindingContext as AdminDashboardViewModel)?.PollAsync();
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
