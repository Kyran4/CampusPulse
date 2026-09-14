using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

[QueryProperty(nameof(EventId), "eventId")]
public class EventDetailsViewModel : BaseViewModel
{
    private readonly EventService _events;
    private readonly DialogService _dialog;

    public EventDetailsViewModel(EventService events, DialogService dialog)
    {
        _events = events;
        _dialog = dialog;

        Attendees = new ObservableCollection<User>();

        JoinCommand = new Command(async () => await JoinEventAsync());
        LeaveCommand = new Command(async () => await LeaveEventAsync());
    }

    private int _eventId;
    public int EventId
    {
        get => _eventId;
        set
        {
            _eventId = value;
            _ = RefreshAsync();
        }
    }

    private Event _event;
    public Event Event
    {
        get => _event;
        private set => SetProperty(ref _event, value);
    }

    public ObservableCollection<User> Attendees { get; }

    // Surfacing this so the page can grey out Join / show "Full" / "Cancelled"
    // instead of letting the user tap it and only find out from an error.
    public bool IsFull => Event?.Capacity.HasValue == true && Attendees.Count >= Event.Capacity.Value;
    public bool IsPast => Event != null && Event.Date < DateTime.UtcNow;
    public bool CanJoin => Event != null && !Event.IsCancelled && !IsPast && !IsFull;

    public ICommand JoinCommand { get; }
    public ICommand LeaveCommand { get; }

    public async Task RefreshAsync()
    {
        IsBusy = true;

        Event = await _events.GetEventAsync(EventId);

        var attendees = await _events.GetAttendeesAsync(EventId);
        Attendees.Clear();

        if (attendees != null)
        {
            foreach (var a in attendees)
                Attendees.Add(a);
        }

        OnPropertyChanged(nameof(Event));
        OnPropertyChanged(nameof(IsFull));
        OnPropertyChanged(nameof(IsPast));
        OnPropertyChanged(nameof(CanJoin));
        IsBusy = false;
    }

    private async Task JoinEventAsync()
    {
        if (!SessionManager.IsLoggedIn)
        {
            await _dialog.ShowAlert("Error", "You must be logged in.");
            return;
        }

        var dto = new EventJoinDto { EventId = EventId };
        var ok = await _events.JoinEventAsync(dto);

        if (!ok)
            await _dialog.ShowAlert("Couldn't join", "This event may be full, cancelled, or already happened.");

        await RefreshAsync();
    }

    private async Task LeaveEventAsync()
    {
        if (!SessionManager.IsLoggedIn)
        {
            await _dialog.ShowAlert("Error", "You must be logged in.");
            return;
        }

        var dto = new EventJoinDto { EventId = EventId };
        await _events.LeaveEventAsync(dto);
        await RefreshAsync();
    }
}
