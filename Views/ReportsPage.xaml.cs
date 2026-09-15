using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class ReportsPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public ReportsPage(ReportsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ReportsViewModel vm)
            vm.RefreshCommand.Execute(null);

        // New reports filed by students, or status changes made by another
        // Admin, while this page is open.
        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => _ = (BindingContext as ReportsViewModel)?.PollAsync();
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
