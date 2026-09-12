using CampusPulse.Models;

namespace CampusPulse.Services;

public class PostService
{
    private readonly ApiClient _api;

    public PostService(ApiClient api)
    {
        _api = api;
    }

    public string? LastError => _api.LastError;

    public async Task<List<Post>?> GetFeedAsync(int? categoryId = null)
    {
        var url = categoryId.HasValue
            ? $"api/posts/feed?categoryId={categoryId.Value}"
            : "api/posts/feed";

        return await _api.GetAsync<List<Post>>(url);
    }

    public async Task<List<Post>?> GetUserPostsAsync(int userId)
    {
        return await _api.GetAsync<List<Post>>($"api/posts/user/{userId}");
    }

    public async Task<Post?> GetPostAsync(int id)
    {
        // There's no GET /api/posts/{id} on the API - the feed and
        // user-posts endpoints are what exist, so pages that need a single
        // post pull it from whichever list already has it. Kept as a
        // fallback that searches the feed by id.
        var feed = await GetFeedAsync();
        return feed?.FirstOrDefault(p => p.PostId == id);
    }

    public async Task<Post?> CreatePostAsync(PostCreateDto dto)
    {
        return await _api.PostAsync<Post>("api/posts", dto);
    }

    public async Task<Post?> UpdatePostAsync(int id, PostCreateDto dto)
    {
        return await _api.PutAsync<Post>($"api/posts/{id}", dto);
    }

    public async Task<bool> DeletePostAsync(int id)
    {
        return await _api.DeleteAsync($"api/posts/{id}");
    }

    public async Task<bool> HidePostAsync(int id, string? reason)
    {
        return await _api.PutAsync($"api/posts/{id}/hide", new ModerationReasonDto { Reason = reason });
    }
}
