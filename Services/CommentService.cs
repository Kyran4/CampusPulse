using CampusPulse.DTOs;
using CampusPulse.Helpers;
using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class CommentService
{
    private readonly SQLiteAsyncConnection _db;

    public CommentService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public Task<List<Comment>> GetCommentsAsync(int postId)
    {
        return _db.Table<Comment>()
            .Where(c => c.PostId == postId)
            .OrderBy(c => c.CreatedDate)
            .ToListAsync();
    }

    public async Task<bool> AddCommentAsync(CommentCreateDto dto)
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return false;

        var comment = new Comment
        {
            PostId = dto.PostId,
            UserId = user.UserId,
            Content = dto.Content
        };

        await _db.InsertAsync(comment);
        return true;
    }

    public async Task<bool> DeleteCommentAsync(int commentId)
    {
        var comment = await _db.Table<Comment>().Where(c => c.CommentId == commentId).FirstOrDefaultAsync();
        var user = SessionManager.CurrentUser;

        if (comment == null || user == null)
            return false;

        if (comment.UserId != user.UserId && user.Role != "Admin")
            return false;

        await _db.DeleteAsync(comment);
        return true;
    }
}
