using CampusPulse.Models;

namespace CampusPulse.Services;

public class CategoryService
{
    private readonly ApiClient _api;

    public CategoryService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<Category>?> GetCategoriesAsync()
    {
        return await _api.GetAsync<List<Category>>("api/categories");
    }
}