using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class MyPostsViewModel : BaseViewModel
{
    private readonly PostService _posts;
    private readonly DatabaseService _db;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public MyPostsViewModel(PostService posts, DatabaseService db, NavigationService nav, DialogService dialog)
    {
        _posts = posts;
        _db = db;
        _nav = nav;
        _dialog = dialog;

        Posts = new ObservableCollection<Post>();
        RefreshCommand = new Command(async () => await LoadMyPostsAsync());
        OpenPostCommand = new Command<Post>(async (p) => await OpenPostAsync(p));
        DeletePostCommand = new Command<Post>(async (p) => await DeletePostAsync(p));

        _ = LoadMyPostsAsync();
    }

    public ObservableCollection<Post> Posts { get; }

    public ICommand RefreshCommand { get; }
    public ICommand OpenPostCommand { get; }
    public ICommand DeletePostCommand { get; }

    private async Task LoadMyPostsAsync()
    {
        IsBusy = true;

        var user = _db.LoadUser<User>();
        if (user == null)
        {
            IsBusy = false;
            return;
        }

        var myPosts = await _posts.GetUserPostsAsync(user.UserId);
        Posts.Clear();

        if (myPosts != null)
        {
            foreach (var post in myPosts)
                Posts.Add(post);
        }

        IsBusy = false;
    }

    public async Task OpenPostAsync(Post post)
    {
        await _nav.GoToAsync($"PostDetailsPage?postId={post.PostId}");
    }

    private async Task DeletePostAsync(Post post)
    {
        if (post == null) return;

        var confirmed = await _dialog.ShowConfirm("Delete Post", "Are you sure you want to delete this post?");
        if (!confirmed) return;

        if (await _posts.DeletePostAsync(post.PostId))
        {
            Posts.Remove(post);
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not delete this post.");
        }
    }
}
