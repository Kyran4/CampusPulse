using CampusPulse.Models;

namespace CampusPulse.Services;

public class UserService
{
    private readonly ApiClient _api;

    public UserService(ApiClient api)
    {
        _api = api;
    }

    public string? LastError => _api.LastError;

    public async Task<User?> GetMeAsync()
    {
        return await _api.GetAsync<User>("api/users/me");
    }

    public async Task<User?> UpdateProfileAsync(UpdateProfileDto dto)
    {
        return await _api.PutAsync<User>("api/users/me", dto);
    }
}
