using CampusPulse.Helpers;
using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class ReactionService
{
    private readonly SQLiteAsyncConnection _db;

    public ReactionService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public async Task<bool> ReactAsync(int postId, string type)
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return false;

        var existing = await _db.Table<Reaction>()
            .Where(r => r.PostId == postId && r.UserId == user.UserId)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            existing.ReactionType = type;
            await _db.UpdateAsync(existing);
        }
        else
        {
            await _db.InsertAsync(new Reaction
            {
                PostId = postId,
                UserId = user.UserId,
                ReactionType = type
            });
        }

        return true;
    }

    public Task<int> CountReactionsAsync(int postId)
    {
        return _db.Table<Reaction>()
            .Where(r => r.PostId == postId)
            .CountAsync();
    }
}
