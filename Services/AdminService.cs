using CampusPulse.Models;

namespace CampusPulse.Services;

public class AdminService
{
    private readonly ApiClient _api;

    public string? LastError => _api.LastError;

    public AdminService(ApiClient api)
    {
        _api = api;
    }

    public async Task<DashboardStatsDto?> GetDashboardAsync()
    {
        return await _api.GetAsync<DashboardStatsDto>("api/admin/dashboard");
    }

    public async Task<List<UserAdminDto>?> GetUsersAsync(string? search = null)
    {
        var url = string.IsNullOrWhiteSpace(search) ? "api/admin/users" : $"api/admin/users?search={Uri.EscapeDataString(search)}";
        return await _api.GetAsync<List<UserAdminDto>>(url);
    }

    public async Task<bool> DeactivateUserAsync(int id)
    {
        return await _api.PutAsync($"api/admin/users/{id}/deactivate", new { });
    }

    public async Task<bool> ReactivateUserAsync(int id)
    {
        return await _api.PutAsync($"api/admin/users/{id}/reactivate", new { });
    }
}
