using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

[QueryProperty(nameof(EditPostId), "editPostId")]
public class CreatePostViewModel : BaseViewModel
{
    private readonly PostService _posts;
    private readonly CategoryService _categories;
    private readonly DatabaseService _db;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public CreatePostViewModel(
        PostService posts,
        CategoryService categories,
        DatabaseService db,
        NavigationService nav,
        DialogService dialog)
    {
        _posts = posts;
        _categories = categories;
        _db = db;
        _nav = nav;
        _dialog = dialog;

        CreateCommand = new Command(async () => await SaveAsync());
    }

    // When set (via the "CreatePostPage?editPostId=..." route from
    // PostDetailsPage), this page edits that post instead of creating a
    // new one. 0/default means "creating a new post".
    private int _editPostId;
    public int EditPostId
    {
        get => _editPostId;
        set
        {
            _editPostId = value;
            if (value > 0)
                _ = LoadPostToEditAsync(value);
        }
    }

    public bool IsEditing => EditPostId > 0;
    public string PageTitle => IsEditing ? "Edit Post" : "Create Post";
    public string SaveButtonText => IsEditing ? "Save Changes" : "Publish Post";

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;

    private List<Category> _categoriesList = new();
    public List<Category> Categories
    {
        get => _categoriesList;
        set => SetProperty(ref _categoriesList, value);
    }

    public Category SelectedCategory { get; set; }

    public ICommand CreateCommand { get; }

    public async Task LoadCategoriesAsync()
    {
        var list = await _categories.GetCategoriesAsync();

        if (list == null)
        {
            // GetCategoriesAsync returning null (as opposed to an empty
            // list) means the request itself failed - most commonly the
            // app couldn't reach the API at all. An empty dropdown with no
            // explanation looks like a broken feature; this at least tells
            // you it's a connection problem, not a missing category.
            Categories = new List<Category>();
            await _dialog.ShowAlert("Error", "Couldn't reach the server to load categories. Check the API is running and reachable from this device.");
            return;
        }

        Categories = list;

        if (IsEditing && _pendingCategoryId.HasValue)
            SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == _pendingCategoryId.Value);
    }

    private int? _pendingCategoryId;

    private async Task LoadPostToEditAsync(int postId)
    {
        var post = await _posts.GetPostAsync(postId);
        if (post == null)
        {
            await _dialog.ShowAlert("Error", "Could not load this post.");
            return;
        }

        Title = post.Title;
        Content = post.Content;
        ImageBase64 = post.ImageBase64 ?? string.Empty;
        _pendingCategoryId = post.CategoryId;

        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(Content));
        OnPropertyChanged(nameof(ImageBase64));
        OnPropertyChanged(nameof(IsEditing));
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(SaveButtonText));

        if (Categories.Count > 0)
            SelectedCategory = Categories.FirstOrDefault(c => c.CategoryId == post.CategoryId);
    }

    private async Task SaveAsync()
    {
        var user = _db.LoadUser<User>();
        if (user == null)
        {
            await _dialog.ShowAlert("Error", "You must be logged in.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Title) ||
            string.IsNullOrWhiteSpace(Content) ||
            SelectedCategory == null)
        {
            await _dialog.ShowAlert("Error", "All fields are required.");
            return;
        }

        var dto = new PostCreateDto
        {
            CategoryId = SelectedCategory.CategoryId,
            Title = Title,
            Content = Content,
            ImageBase64 = ImageBase64
        };

        var result = IsEditing
            ? await _posts.UpdatePostAsync(EditPostId, dto)
            : await _posts.CreatePostAsync(dto);

        if (result == null)
        {
            var detail = string.IsNullOrWhiteSpace(_posts.LastError) ? "" : $"\n\n({_posts.LastError})";
            await _dialog.ShowAlert("Error", (IsEditing ? "Failed to update post." : "Failed to create post.") + detail);
            return;
        }

        await _dialog.ShowAlert("Success", IsEditing ? "Post updated!" : "Post created!");
        await _nav.GoBackAsync();
    }
}
