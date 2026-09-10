using System.Net.Http.Json;
using CampusPulse.Models;

namespace CampusPulse.Services;

public class CategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService()
    {
        var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5012/"
            : "http://localhost:5012/";

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public async Task<List<Category>> GetInterestsAsync()
    {
        var interests =
            await _httpClient.GetFromJsonAsync<List<Category>>("api/interests");

        return interests ?? new List<Category>();
    }

    public async Task<bool> FollowInterestAsync(int interestId, int userId)
    {
        var response = await _httpClient.PostAsync(
            $"api/interests/{interestId}/follow/{userId}",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UnfollowInterestAsync(int interestId, int userId)
    {
        var response = await _httpClient.DeleteAsync(
            $"api/interests/{interestId}/follow/{userId}");

        return response.IsSuccessStatusCode;
    }

    public async Task<List<Category>> GetFollowedInterestsAsync(int userId)
    {
        var interests =
            await _httpClient.GetFromJsonAsync<List<Category>>(
                $"api/interests/followed/{userId}");

        return interests ?? new List<Category>();
    }
}