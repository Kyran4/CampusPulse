using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class EventsViewModel : BaseViewModel
{
    private readonly EventService _events;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public EventsViewModel(EventService events, NavigationService nav, DialogService dialog)
    {
        _events = events;
        _nav = nav;
        _dialog = dialog;

        Events = new ObservableCollection<Event>();
        RefreshCommand = new Command(async () => await LoadEventsAsync());
        CreateEventCommand = new Command(async () => await _nav.GoToAsync("CreateEventPage"));
        OpenEventCommand = new Command<Event>(async (e) => await OpenEventAsync(e));
        CancelEventCommand = new Command<Event>(async (e) => await CancelEventAsync(e));

        _ = LoadEventsAsync();
    }

    public ObservableCollection<Event> Events { get; }

    // Drives visibility of the "Create Event" button and each event's
    // "Cancel" button - Admin only, checked again server-side regardless.
    public bool IsAdmin => SessionManager.IsAdmin;

    public ICommand RefreshCommand { get; }
    public ICommand CreateEventCommand { get; }
    public ICommand OpenEventCommand { get; }
    public ICommand CancelEventCommand { get; }

    private async Task LoadEventsAsync()
    {
        IsBusy = true;

        var list = await _events.GetEventsAsync();
        Events.Clear();

        if (list != null)
        {
            foreach (var e in list)
                Events.Add(e);
        }

        OnPropertyChanged(nameof(IsAdmin));
        IsBusy = false;
    }

    public async Task OpenEventAsync(Event ev)
    {
        if (ev == null) return;

        await _nav.GoToAsync($"EventDetailsPage?eventId={ev.EventId}");
    }

    private async Task CancelEventAsync(Event ev)
    {
        if (ev == null) return;

        var confirmed = await _dialog.ShowConfirm("Cancel Event", $"Cancel \"{ev.Title}\"? Registered students will see it as cancelled.");
        if (!confirmed) return;

        if (await _events.CancelEventAsync(ev.EventId))
        {
            ev.IsCancelled = true;
            await LoadEventsAsync();
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not cancel this event.");
        }
    }
}
