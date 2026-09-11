using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class FeedViewModel : BaseViewModel
{
    private readonly PostService _posts;
    private readonly CategoryService _categories;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public FeedViewModel(PostService posts, CategoryService categories, NavigationService nav, DialogService dialog)
    {
        _posts = posts;
        _categories = categories;
        _nav = nav;
        _dialog = dialog;

        Posts = new ObservableCollection<Post>();
        Categories = new ObservableCollection<Category>();

        RefreshCommand = new Command(async () => await LoadFeedAsync());
        CreatePostCommand = new Command(async () => await _nav.GoToAsync("CreatePostPage"));
        OpenPostCommand = new Command<Post>(async (post) => await OpenPostAsync(post));

        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await LoadCategoriesAsync();
        await LoadFeedAsync();
    }

    public ObservableCollection<Post> Posts { get; }

    // US-10: filter the feed by category. Categories come from the
    // Category table (via CategoryService), not a hardcoded list, and
    // "All" (null selection) shows everything.
    public ObservableCollection<Category> Categories { get; }

    private Category _selectedCategory;
    public Category SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
                _ = LoadFeedAsync();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand CreatePostCommand { get; }
    public ICommand OpenPostCommand { get; }

    private async Task LoadCategoriesAsync()
    {
        var list = await _categories.GetCategoriesAsync();
        Categories.Clear();

        // Sentinel "All Categories" entry (CategoryId 0 never exists in the
        // real table) so the picker has a way back to an unfiltered feed.
        Categories.Add(new Category { CategoryId = 0, Name = "All Categories" });

        if (list != null)
        {
            foreach (var c in list)
                Categories.Add(c);
        }

        SelectedCategory = Categories.First();
    }

    private async Task LoadFeedAsync()
    {
        IsBusy = true;

        var categoryId = SelectedCategory != null && SelectedCategory.CategoryId != 0
            ? SelectedCategory.CategoryId
            : (int?)null;

        var feed = await _posts.GetFeedAsync(categoryId);
        Posts.Clear();

        if (feed != null)
        {
            foreach (var post in feed)
                Posts.Add(post);
        }

        IsBusy = false;
    }

    private async Task OpenPostAsync(Post post)
    {
        if (post == null)
            return;

        await _nav.GoToAsync($"PostDetailsPage?postId={post.PostId}");
    }
}
