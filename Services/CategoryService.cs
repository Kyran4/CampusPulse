using CampusPulse.Helpers;
using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class CategoryService
{
    private readonly SQLiteAsyncConnection _db;

    public CategoryService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public Task<List<Category>> GetCategoriesAsync()
    {
        return _db.Table<Category>().ToListAsync();
    }

    public async Task<bool> AddCategoryAsync(string name)
    {
        var admin = SessionManager.CurrentUser;
        if (admin == null || admin.Role != "Admin")
            return false;

        await _db.InsertAsync(new Category { Name = name });
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var admin = SessionManager.CurrentUser;
        if (admin == null || admin.Role != "Admin")
            return false;

        var cat = await _db.Table<Category>().Where(c => c.CategoryId == id).FirstOrDefaultAsync();
        if (cat == null) return false;

        await _db.DeleteAsync(cat);
        return true;
    }
}
