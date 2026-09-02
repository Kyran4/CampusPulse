using CampusPulse.DTOs;
using CampusPulse.Helpers;
using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class AuthenticationService
{
    private readonly SQLiteAsyncConnection _db;

    public AuthenticationService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public async Task<User> LoginAsync(LoginRequest request)
    {
        var user = await _db.Table<User>()
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (user == null)
            return null;

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        if (!user.IsActive)
            return null;

        SessionManager.CurrentUser = user;
        return user;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var exists = await _db.Table<User>()
            .Where(u => u.Email == request.Email)
            .FirstOrDefaultAsync();

        if (exists != null)
            return false;

        var newUser = new User
        {
            DisplayName = request.DisplayName,
            Email = request.Email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = "Student",
            IsActive = true
        };

        await _db.InsertAsync(newUser);
        return true;
    }

    public void Logout()
    {
        SessionManager.CurrentUser = null;
    }
}
