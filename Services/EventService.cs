using CampusPulse.Models;

namespace CampusPulse.Services;

public class EventService
{
    private readonly ApiClient _api;

    public EventService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<Event>?> GetEventsAsync()
    {
        return await _api.GetAsync<List<Event>>("api/events");
    }

    public async Task<Event?> GetEventAsync(int id)
    {
        return await _api.GetAsync<Event>($"api/events/{id}");
    }

    public async Task<List<User>?> GetAttendeesAsync(int eventId)
    {
        return await _api.GetAsync<List<User>>($"api/events/{eventId}/attendees");
    }

    public async Task<bool> JoinEventAsync(EventJoinDto dto)
    {
        return await _api.PostAsync("api/events/join", dto);
    }

    public async Task<bool> LeaveEventAsync(EventJoinDto dto)
    {
        return await _api.PostAsync("api/events/leave", dto);
    }

    // ---- Admin ----

    public async Task<Event?> CreateEventAsync(EventCreateDto dto)
    {
        return await _api.PostAsync<Event>("api/events", dto);
    }

    public async Task<Event?> UpdateEventAsync(int id, EventCreateDto dto)
    {
        return await _api.PutAsync<Event>($"api/events/{id}", dto);
    }

    public async Task<bool> CancelEventAsync(int id)
    {
        return await _api.PutAsync($"api/events/{id}/cancel", new { });
    }
}
