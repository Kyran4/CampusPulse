using CampusPulse.Models;

namespace CampusPulse.Services;

public class UserService
{
    private readonly ApiClient _api;

    public UserService(ApiClient api)
    {
        _api = api;
    }

    public async Task<User?> GetUserAsync(int id)
    {
        return await _api.GetAsync<User>($"users/{id}");
    }
}
