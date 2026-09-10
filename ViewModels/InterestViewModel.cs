using CampusPulse.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class InterestsViewModel : INotifyPropertyChanged
{
    private readonly CategoryService _categoryService;

    private const int UserId = 1;

    public ObservableCollection<InterestItemViewModel> Interests { get; set; }

    public ObservableCollection<InterestItemViewModel> FollowedInterests { get; set; }

    public ICommand ToggleFollowCommand { get; }

    public InterestsViewModel()
    {
        _categoryService = new CategoryService();

        Interests = new ObservableCollection<InterestItemViewModel>();
        FollowedInterests = new ObservableCollection<InterestItemViewModel>();

        ToggleFollowCommand =
            new Command<InterestItemViewModel>(
                async interest => await ToggleFollowAsync(interest));
    }

    public async Task LoadInterestsAsync()
    {
        try
        {
            Interests.Clear();
            FollowedInterests.Clear();

            var interests = await _categoryService.GetInterestsAsync();
            var followed = await _categoryService.GetFollowedInterestsAsync(UserId);

            foreach (var category in interests)
            {
                var isFollowing =
                    followed.Any(f => f.CategoryId == category.CategoryId);

                var item = new InterestItemViewModel
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    IsFollowing = isFollowing
                };

                Interests.Add(item);

                if (isFollowing)
                {
                    FollowedInterests.Add(item);
                }
            }

            UpdateFollowingStatus();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"Could not load interests: {ex.Message}",
                "OK");
        }
    }

    private async Task ToggleFollowAsync(InterestItemViewModel interest)
    {
        if (interest == null)
            return;

        bool success;

        if (interest.IsFollowing)
        {
            success = await _categoryService
                .UnfollowInterestAsync(interest.CategoryId, UserId);

            if (success)
            {
                interest.IsFollowing = false;
                FollowedInterests.Remove(interest);
            }
        }
        else
        {
            success = await _categoryService
                .FollowInterestAsync(interest.CategoryId, UserId);

            if (success)
            {
                interest.IsFollowing = true;

                if (!FollowedInterests.Contains(interest))
                {
                    FollowedInterests.Add(interest);
                }
            }
        }

        UpdateFollowingStatus();
    }

    private void UpdateFollowingStatus()
    {
        OnPropertyChanged(nameof(HasFollowedInterests));
        OnPropertyChanged(nameof(HasNoFollowedInterests));
    }

    public bool HasFollowedInterests =>
        FollowedInterests.Count > 0;

    public bool HasNoFollowedInterests =>
        FollowedInterests.Count == 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}

public class InterestItemViewModel : INotifyPropertyChanged
{
    private bool _isFollowing;

    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool IsFollowing
    {
        get => _isFollowing;

        set
        {
            if (_isFollowing == value)
                return;

            _isFollowing = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(FollowButtonText));
        }
    }

    public string FollowButtonText =>
        IsFollowing ? "Unfollow" : "Follow";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}