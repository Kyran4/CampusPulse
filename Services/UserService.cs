using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class UserService
{
    private readonly SQLiteAsyncConnection _db;

    public UserService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public Task<User> GetUserAsync(int id)
    {
        return _db.Table<User>().Where(u => u.UserId == id).FirstOrDefaultAsync();
    }

    public Task<List<User>> GetAllUsersAsync()
    {
        return _db.Table<User>().ToListAsync();
    }

    public async Task<bool> UpdateProfileAsync(User user)
    {
        await _db.UpdateAsync(user);
        return true;
    }

    public async Task<bool> SetActiveStatusAsync(int userId, bool isActive)
    {
        var user = await GetUserAsync(userId);
        if (user == null) return false;

        user.IsActive = isActive;
        await _db.UpdateAsync(user);
        return true;
    }
}
