using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class AdminDashboardViewModel : BaseViewModel
{
    private readonly PostService _posts;
    private readonly AdminService _admin;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public AdminDashboardViewModel(PostService posts, AdminService admin, NavigationService nav, DialogService dialog)
    {
        _posts = posts;
        _admin = admin;
        _nav = nav;
        _dialog = dialog;

        Posts = new ObservableCollection<Post>();
        Stats = new DashboardStatsDto();

        RefreshCommand = new Command(async () => await LoadAsync());
        OpenPostCommand = new Command<Post>(async (p) => await OpenPostAsync(p));
        // This command was already referenced by AdminDashboardPage.xaml's
        // "Hide" button but had no matching command here - the button
        // silently did nothing. Wiring it up now.
        HidePostCommand = new Command<Post>(async (p) => await HidePostAsync(p));

        _ = LoadAsync();
    }

    public ObservableCollection<Post> Posts { get; }

    private DashboardStatsDto _stats;
    public DashboardStatsDto Stats
    {
        get => _stats;
        private set => SetProperty(ref _stats, value);
    }

    public ICommand RefreshCommand { get; }
    public ICommand OpenPostCommand { get; }
    public ICommand HidePostCommand { get; }

    private async Task LoadAsync()
    {
        IsBusy = true;

        var stats = await _admin.GetDashboardAsync();
        if (stats != null)
        {
            Stats = stats;
        }
        else
        {
            // Distinguishing this from "genuinely zero" - if you see this,
            // it's a permissions/connection problem (this endpoint requires
            // a valid Admin token), not an empty database.
            await _dialog.ShowAlert("Error", "Couldn't load dashboard stats. Make sure you're logged in as an Admin and the API is reachable.");
        }

        var feed = await _posts.GetFeedAsync();
        Posts.Clear();

        if (feed != null)
        {
            foreach (var p in feed)
                Posts.Add(p);
        }

        IsBusy = false;
    }

    public async Task OpenPostAsync(Post post)
    {
        await _nav.GoToAsync($"PostDetailsPage?postId={post.PostId}");
    }

    private async Task HidePostAsync(Post post)
    {
        if (post == null) return;

        var reason = await Application.Current.MainPage.DisplayPromptAsync(
            "Hide Post",
            "Reason for hiding this post (shown in the moderation log):");

        // Confirming with an empty reason still hides it - Section 10.4
        // only says "where possible", so we don't hard-block on it, but we
        // do prompt for one every time.
        if (await _posts.HidePostAsync(post.PostId, reason))
        {
            Posts.Remove(post);
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not hide this post.");
        }
    }
}
