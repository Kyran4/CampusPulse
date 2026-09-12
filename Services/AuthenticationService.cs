using CampusPulse.Models;
using CampusPulse.Services;

namespace CampusPulse.Services;

public class AuthenticationService
{
    private readonly ApiClient _api;

    public AuthenticationService(ApiClient api)
    {
        _api = api;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequest dto)
    {
        return await _api.PostAsync<AuthResponseDto>("api/auth/login", dto);
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequest dto)
    {
        return await _api.PostAsync<AuthResponseDto>("api/auth/register", dto);
    }
}