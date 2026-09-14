using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class EventsPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public EventsPage(EventsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EventsViewModel vm)
            vm.RefreshCommand.Execute(null);

        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => (BindingContext as EventsViewModel)?.RefreshCommand.Execute(null);
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
