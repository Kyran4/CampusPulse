using CampusPulse.DTOs;
using CampusPulse.Helpers;
using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class PostService
{
    private readonly SQLiteAsyncConnection _db;

    public PostService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public Task<List<Post>> GetFeedAsync()
    {
        return _db.Table<Post>()
            .Where(p => !p.IsHidden)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public Task<List<Post>> GetPostsByUserAsync(int userId)
    {
        return _db.Table<Post>()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();
    }

    public async Task<bool> CreatePostAsync(PostCreateDto dto)
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return false;

        var post = new Post
        {
            UserId = user.UserId,
            Title = dto.Title,
            Content = dto.Content,
            CategoryId = dto.CategoryId
        };

        await _db.InsertAsync(post);
        return true;
    }

    public async Task<bool> UpdatePostAsync(Post post)
    {
        var user = SessionManager.CurrentUser;
        if (user == null || user.UserId != post.UserId)
            return false;

        post.UpdatedDate = DateTime.UtcNow;
        await _db.UpdateAsync(post);
        return true;
    }

    public async Task<bool> DeletePostAsync(int postId)
    {
        var post = await _db.Table<Post>().Where(p => p.PostId == postId).FirstOrDefaultAsync();
        var user = SessionManager.CurrentUser;

        if (post == null || user == null)
            return false;

        if (post.UserId != user.UserId && user.Role != "Admin")
            return false;

        await _db.DeleteAsync(post);
        return true;
    }
}
