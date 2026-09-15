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
    private readonly FollowService _follows;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public FeedViewModel(PostService posts, CategoryService categories, FollowService follows, NavigationService nav, DialogService dialog)
    {
        _posts = posts;
        _categories = categories;
        _follows = follows;
        _nav = nav;
        _dialog = dialog;

        Posts = new ObservableCollection<Post>();
        Categories = new ObservableCollection<Category>();

        RefreshCommand = new Command(async () => await LoadFeedAsync());
        CreatePostCommand = new Command(async () => await _nav.GoToAsync("CreatePostPage"));
        OpenPostCommand = new Command<Post>(async (post) => await OpenPostAsync(post));
        ManageInterestsCommand = new Command(async () => await _nav.GoToAsync("InterestsPage"));

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

    // Change request: "a filter showing posts from followed categories" -
    // this and the category picker are mutually exclusive filters on the
    // same feed rather than two separate screens. Turning this on ignores
    // SelectedCategory; turning it off goes back to the normal/category feed.
    private bool _showFollowingOnly;
    public bool ShowFollowingOnly
    {
        get => _showFollowingOnly;
        set
        {
            if (SetProperty(ref _showFollowingOnly, value))
                _ = LoadFeedAsync();
        }
    }

    public ICommand RefreshCommand { get; }
    public ICommand CreatePostCommand { get; }
    public ICommand OpenPostCommand { get; }
    public ICommand ManageInterestsCommand { get; }

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

    private async Task LoadFeedAsync(bool showErrorAlert = true)
    {
        IsBusy = true;

        List<Post>? feed;
        string? error = null;

        if (ShowFollowingOnly)
        {
            feed = await _follows.GetFollowingFeedAsync();
            error = _follows.LastError;
        }
        else
        {
            var categoryId = SelectedCategory != null && SelectedCategory.CategoryId != 0
                ? SelectedCategory.CategoryId
                : (int?)null;

            feed = await _posts.GetFeedAsync(categoryId);
            error = _posts.LastError;
        }

        // Previously this branch never checked for failure at all - a
        // failed request just silently cleared the list with no
        // explanation, which is exactly what "posts don't show up, no
        // error, nothing" looks like. showErrorAlert is false during the
        // background poll (see FeedPage's timer) so a dropped connection
        // doesn't pop up a dialog every 7 seconds - it only surfaces loudly
        // on an explicit load (first open, pull-to-refresh, changing the
        // category filter).
        if (feed == null && showErrorAlert)
        {
            await _dialog.ShowAlert("Error", "Couldn't load the feed." + (string.IsNullOrWhiteSpace(error) ? "" : $"\n\n({error})"));
        }

        Posts.Clear();

        if (feed != null)
        {
            foreach (var post in feed)
                Posts.Add(post);
        }

        IsBusy = false;
    }

    // Called by the background poll timer - same reload, but failures are
    // logged (via ApiClient.LastError/Console) rather than shown, since a
    // silent poll failing shouldn't interrupt whatever the user's doing.
    public async Task PollAsync() => await LoadFeedAsync(showErrorAlert: false);

    private async Task OpenPostAsync(Post post)
    {
        if (post == null)
            return;

        await _nav.GoToAsync($"PostDetailsPage?postId={post.PostId}");
    }
}
