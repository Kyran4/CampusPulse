using CampusPulse.Models;

namespace CampusPulse.Services;

public class CommentService
{
    private readonly ApiClient _api;

    public CommentService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<Comment>?> GetCommentsAsync(int postId)
    {
        return await _api.GetAsync<List<Comment>>($"api/comments/{postId}");
    }

    public async Task<Comment?> CreateCommentAsync(CommentCreateDto dto)
    {
        return await _api.PostAsync<Comment>("api/comments", dto);
    }

    public async Task<Comment?> UpdateCommentAsync(int id, CommentUpdateDto dto)
    {
        return await _api.PutAsync<Comment>($"api/comments/{id}", dto);
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        return await _api.DeleteAsync($"api/comments/{id}");
    }

    public async Task<bool> HideCommentAsync(int id, string? reason)
    {
        return await _api.PutAsync($"api/comments/{id}/hide", new ModerationReasonDto { Reason = reason });
    }
}
