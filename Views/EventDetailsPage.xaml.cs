using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class EventDetailsPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public EventDetailsPage(EventDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // So other students joining/leaving shows up in the attendee list
        // and capacity while you're looking at the event.
        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => _ = (BindingContext as EventDetailsViewModel)?.RefreshAsync();
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
