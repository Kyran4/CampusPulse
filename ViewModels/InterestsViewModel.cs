using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

// Wraps a Category with whether the current user follows it, so the
// Follow/Unfollow button's visibility and behaviour can bind directly per
// row instead of the page having to cross-reference two separate lists.
public class InterestDisplay
{
    public Category Category { get; set; }
    public bool IsFollowed { get; set; }

    public int CategoryId => Category.CategoryId;
    public string Name => Category.Name;
}

public class InterestsViewModel : BaseViewModel
{
    private readonly CategoryService _categories;
    private readonly FollowService _follows;
    private readonly DialogService _dialog;

    public InterestsViewModel(CategoryService categories, FollowService follows, DialogService dialog)
    {
        _categories = categories;
        _follows = follows;
        _dialog = dialog;

        Interests = new ObservableCollection<InterestDisplay>();

        RefreshCommand = new Command(async () => await LoadAsync());
        FollowCommand = new Command<InterestDisplay>(async (i) => await FollowAsync(i));
        UnfollowCommand = new Command<InterestDisplay>(async (i) => await UnfollowAsync(i));

        _ = LoadAsync();
    }

    // AC: "A logged-in student can view available interests" and "can view
    // their currently followed interests" - one list, each row shows its
    // own follow state, rather than two separate screens.
    public ObservableCollection<InterestDisplay> Interests { get; }

    public ICommand RefreshCommand { get; }
    public ICommand FollowCommand { get; }
    public ICommand UnfollowCommand { get; }

    private async Task LoadAsync()
    {
        IsBusy = true;

        var all = await _categories.GetCategoriesAsync();
        var followed = await _follows.GetFollowedCategoriesAsync();

        Interests.Clear();

        if (all == null)
        {
            await _dialog.ShowAlert("Error", "Couldn't load interests. Check the API is running and reachable.");
            IsBusy = false;
            return;
        }

        var followedIds = (followed ?? new List<Category>()).Select(c => c.CategoryId).ToHashSet();

        foreach (var category in all)
        {
            Interests.Add(new InterestDisplay
            {
                Category = category,
                IsFollowed = followedIds.Contains(category.CategoryId)
            });
        }

        IsBusy = false;
    }

    private async Task FollowAsync(InterestDisplay interest)
    {
        if (interest == null) return;

        if (await _follows.FollowAsync(interest.CategoryId))
        {
            await LoadAsync();
        }
        else
        {
            await _dialog.ShowAlert("Error", "Couldn't follow this interest." + (string.IsNullOrWhiteSpace(_follows.LastError) ? "" : $"\n\n({_follows.LastError})"));
        }
    }

    private async Task UnfollowAsync(InterestDisplay interest)
    {
        if (interest == null) return;

        if (await _follows.UnfollowAsync(interest.CategoryId))
        {
            await LoadAsync();
        }
        else
        {
            await _dialog.ShowAlert("Error", "Couldn't unfollow this interest." + (string.IsNullOrWhiteSpace(_follows.LastError) ? "" : $"\n\n({_follows.LastError})"));
        }
    }
}
