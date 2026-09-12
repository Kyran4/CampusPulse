using CampusPulse.Models;

namespace CampusPulse.Services;

public class FollowService
{
    private readonly ApiClient _api;

    public FollowService(ApiClient api)
    {
        _api = api;
    }

    public string? LastError => _api.LastError;

    public async Task<List<Category>?> GetFollowedCategoriesAsync()
    {
        return await _api.GetAsync<List<Category>>("api/follows");
    }

    public async Task<bool> FollowAsync(int categoryId)
    {
        return await _api.PostAsync("api/follows", new FollowCategoryDto { CategoryId = categoryId });
    }

    public async Task<bool> UnfollowAsync(int categoryId)
    {
        return await _api.DeleteAsync($"api/follows/{categoryId}");
    }

    public async Task<List<Post>?> GetFollowingFeedAsync()
    {
        return await _api.GetAsync<List<Post>>("api/posts/following");
    }
}
