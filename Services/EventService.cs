using CampusPulse.DTOs;
using CampusPulse.Helpers;
using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class EventService
{
    private readonly SQLiteAsyncConnection _db;

    public EventService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public Task<List<Event>> GetEventsAsync()
    {
        return _db.Table<Event>()
            .OrderBy(e => e.Date)
            .ToListAsync();
    }

    public async Task<bool> CreateEventAsync(EventCreateDto dto)
    {
        var user = SessionManager.CurrentUser;
        if (user == null || user.Role != "Admin")
            return false;

        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Date = dto.Date,
            Location = dto.Location,
            CategoryId = dto.CategoryId,
            Capacity = dto.Capacity,
            CreatedBy = user.UserId
        };

        await _db.InsertAsync(ev);
        return true;
    }

    public async Task<bool> JoinEventAsync(int eventId)
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return false;

        var exists = await _db.Table<EventRegistration>()
            .Where(r => r.EventId == eventId && r.UserId == user.UserId)
            .FirstOrDefaultAsync();

        if (exists != null)
            return false;

        await _db.InsertAsync(new EventRegistration
        {
            EventId = eventId,
            UserId = user.UserId
        });

        return true;
    }

    public async Task<bool> LeaveEventAsync(int eventId)
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return false;

        var reg = await _db.Table<EventRegistration>()
            .Where(r => r.EventId == eventId && r.UserId == user.UserId)
            .FirstOrDefaultAsync();

        if (reg == null)
            return false;

        await _db.DeleteAsync(reg);
        return true;
    }
}
