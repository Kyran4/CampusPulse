using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

// Wraps a Comment with a pre-computed "can the current user manage this"
// flag, so the Delete button's visibility can bind directly instead of
// needing a converter that reaches into SessionManager from XAML.
public class CommentDisplay
{
    public Comment Comment { get; set; }
    public bool CanManage { get; set; }

    public int CommentId => Comment.CommentId;
    public string Content => Comment.Content;
}

[QueryProperty(nameof(PostId), "postId")]
public class PostDetailsViewModel : BaseViewModel
{
    private readonly PostService _posts;
    private readonly CommentService _comments;
    private readonly ReactionService _reactions;
    private readonly ReportService _reports;
    private readonly DatabaseService _db;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public PostDetailsViewModel(
        PostService posts,
        CommentService comments,
        ReactionService reactions,
        ReportService reports,
        DatabaseService db,
        NavigationService nav,
        DialogService dialog)
    {
        _posts = posts;
        _comments = comments;
        _reactions = reactions;
        _reports = reports;
        _db = db;
        _nav = nav;
        _dialog = dialog;

        Comments = new ObservableCollection<CommentDisplay>();

        AddCommentCommand = new Command(async () => await AddCommentAsync());
        DeleteCommentCommand = new Command<CommentDisplay>(async (c) => await DeleteCommentAsync(c));
        AddReactionCommand = new Command<string>(async (type) => await AddReactionAsync(type));
        EditPostCommand = new Command(async () => await EditPostAsync());
        DeletePostCommand = new Command(async () => await DeletePostAsync());
        ReportPostCommand = new Command(async () => await ReportPostAsync());
    }

    private int _postId;
    public int PostId
    {
        get => _postId;
        set
        {
            _postId = value;
            _ = RefreshAsync();
        }
    }

    private Post _post;
    public Post Post
    {
        get => _post;
        private set => SetProperty(ref _post, value);
    }

    public ObservableCollection<CommentDisplay> Comments { get; }

    public string ReactionSummary { get; private set; } = string.Empty;

    public string NewComment { get; set; } = string.Empty;

    // Drives IsVisible on the Edit/Delete buttons in the page - only the
    // post's owner (or an Admin) should ever see them. The API re-checks
    // ownership regardless, but there's no reason to show a button the
    // server will just reject.
    public bool CanManagePost =>
        Post != null && SessionManager.CurrentUser != null &&
        (Post.UserId == SessionManager.CurrentUser.UserId || SessionManager.IsAdmin);

    // Admins moderate directly (Hide/Delete) rather than filing a report
    // against their own review queue - Reporting is a Student-facing action.
    public bool CanReport => !SessionManager.IsAdmin;

    public ICommand AddCommentCommand { get; }
    public ICommand DeleteCommentCommand { get; }
    public ICommand AddReactionCommand { get; }
    public ICommand EditPostCommand { get; }
    public ICommand DeletePostCommand { get; }
    public ICommand ReportPostCommand { get; }

    public async Task RefreshAsync()
    {
        IsBusy = true;

        Post = await _posts.GetPostAsync(PostId);
        var comments = await _comments.GetCommentsAsync(PostId);
        var reactions = await _reactions.GetReactionsAsync(PostId);

        Comments.Clear();
        if (comments != null)
        {
            var currentUserId = SessionManager.CurrentUser?.UserId;
            foreach (var c in comments)
            {
                Comments.Add(new CommentDisplay
                {
                    Comment = c,
                    CanManage = currentUserId.HasValue && (c.UserId == currentUserId.Value || SessionManager.IsAdmin)
                });
            }
        }

        // Without this the reaction buttons had no visible effect even when
        // they worked - tapping "Like" did something on the server but
        // nothing on screen ever changed, which reads as "does nothing".
        if (reactions != null && reactions.Count > 0)
        {
            var likeCount = reactions.Count(r => r.Type == ReactionService.Like);
            var helpfulCount = reactions.Count(r => r.Type == ReactionService.Helpful);
            var interestedCount = reactions.Count(r => r.Type == ReactionService.Interested);
            var myUserId = SessionManager.CurrentUser?.UserId;
            var mine = myUserId.HasValue ? reactions.FirstOrDefault(r => r.UserId == myUserId.Value) : null;

            ReactionSummary = $"👍 {likeCount}   🙌 {helpfulCount}   👀 {interestedCount}" +
                (mine != null ? $"   (you reacted: {mine.Type})" : "");
        }
        else
        {
            ReactionSummary = "No reactions yet.";
        }

        OnPropertyChanged(nameof(Post));
        OnPropertyChanged(nameof(CanManagePost));
        OnPropertyChanged(nameof(CanReport));
        OnPropertyChanged(nameof(ReactionSummary));
        IsBusy = false;
    }

    private async Task AddCommentAsync()
    {
        if (!SessionManager.IsLoggedIn)
        {
            await _dialog.ShowAlert("Error", "You must be logged in.");
            return;
        }

        if (string.IsNullOrWhiteSpace(NewComment))
        {
            await _dialog.ShowAlert("Error", "Comment cannot be empty.");
            return;
        }

        var dto = new CommentCreateDto
        {
            PostId = PostId,
            Content = NewComment
        };

        var comment = await _comments.CreateCommentAsync(dto);
        if (comment != null)
        {
            var currentUserId = SessionManager.CurrentUser?.UserId;
            Comments.Insert(0, new CommentDisplay
            {
                Comment = comment,
                CanManage = currentUserId.HasValue && (comment.UserId == currentUserId.Value || SessionManager.IsAdmin)
            });
            NewComment = string.Empty;
            OnPropertyChanged(nameof(NewComment));
        }
        else
        {
            await _dialog.ShowAlert("Error", "Failed to post comment.");
        }
    }

    private async Task DeleteCommentAsync(CommentDisplay comment)
    {
        if (comment == null) return;

        var confirmed = await _dialog.ShowConfirm("Delete Comment", "Remove this comment?");
        if (!confirmed) return;

        if (await _comments.DeleteCommentAsync(comment.CommentId))
        {
            Comments.Remove(comment);
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not delete comment.");
        }
    }

    // US-13: one reaction per post per user. Reacting with the same type
    // again removes it; a different type replaces it - both handled by the
    // API, we just reload afterwards.
    private async Task AddReactionAsync(string type)
    {
        if (!SessionManager.IsLoggedIn)
        {
            await _dialog.ShowAlert("Error", "You must be logged in.");
            return;
        }

        var dto = new ReactionCreateDto
        {
            PostId = PostId,
            Type = type
        };

        var success = await _reactions.AddReactionAsync(dto);

        if (!success)
        {
            await _dialog.ShowAlert("Error", "Couldn't record your reaction. Please try again.");
            return;
        }

        await RefreshAsync();
    }

    private async Task EditPostAsync()
    {
        if (Post == null) return;
        await _nav.GoToAsync($"CreatePostPage?editPostId={Post.PostId}");
    }

    private async Task DeletePostAsync()
    {
        if (Post == null) return;

        var confirmed = await _dialog.ShowConfirm("Delete Post", "Are you sure you want to delete this post? This cannot be undone.");
        if (!confirmed) return;

        if (await _posts.DeletePostAsync(Post.PostId))
        {
            await _dialog.ShowAlert("Deleted", "Your post has been removed.");
            await _nav.GoBackAsync();
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not delete this post.");
        }
    }

    // US-19: any student can report a post, with a required reason. This
    // never removes the content directly - it just files a Pending report
    // for an Admin to review.
    private async Task ReportPostAsync()
    {
        if (Post == null) return;

        if (!SessionManager.IsLoggedIn)
        {
            await _dialog.ShowAlert("Error", "You must be logged in.");
            return;
        }

        var reason = await Application.Current.MainPage.DisplayPromptAsync(
            "Report Post",
            "Why are you reporting this post?");

        if (string.IsNullOrWhiteSpace(reason))
            return;

        var dto = new ReportCreateDto
        {
            PostId = Post.PostId,
            Reason = reason
        };

        var result = await _reports.CreateReportAsync(dto);

        await _dialog.ShowAlert(
            result != null ? "Reported" : "Error",
            result != null
                ? "Thanks - an admin will review this."
                : "Could not submit the report. Please try again.");
    }
}
