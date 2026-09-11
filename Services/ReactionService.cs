using CampusPulse.Models;

namespace CampusPulse.Services;

public class ReactionService
{
    public const string Like = "Like";
    public const string Helpful = "Helpful";
    public const string Interested = "Interested";

    private readonly ApiClient _api;

    public ReactionService(ApiClient api)
    {
        _api = api;
    }

    // Posting the same type again toggles it off server-side; a different
    // type replaces the previous one. Either way the caller should reload
    // the post afterwards to see the updated reaction state.
    public async Task<bool> AddReactionAsync(ReactionCreateDto dto)
    {
        return await _api.PostAsync("api/reactions", dto);
    }

    public async Task<List<Reaction>?> GetReactionsAsync(int postId)
    {
        return await _api.GetAsync<List<Reaction>>($"api/reactions/{postId}");
    }
}
