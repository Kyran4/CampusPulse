using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class InterestsViewModel
{
    public ObservableCollection<InterestItemViewModel> Interests { get; set; }

    public ICommand ToggleFollowCommand { get; }

    public InterestsViewModel()
    {
        Interests = new ObservableCollection<InterestItemViewModel>
        {
            new InterestItemViewModel { CategoryId = 1, Name = "Sports" },
            new InterestItemViewModel { CategoryId = 2, Name = "Technology" },
            new InterestItemViewModel { CategoryId = 3, Name = "Gaming" },
            new InterestItemViewModel { CategoryId = 4, Name = "Music" },
            new InterestItemViewModel { CategoryId = 5, Name = "Cultural Events" },
            new InterestItemViewModel { CategoryId = 6, Name = "Careers" },
            new InterestItemViewModel { CategoryId = 7, Name = "Study Support" },
            new InterestItemViewModel { CategoryId = 8, Name = "Volunteering" }
        };

        ToggleFollowCommand = new Command<InterestItemViewModel>(ToggleFollow);
    }

    private void ToggleFollow(InterestItemViewModel interest)
    {
        if (interest == null)
            return;

        interest.IsFollowing = !interest.IsFollowing;
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

    public string FollowButtonText => IsFollowing ? "Unfollow" : "Follow";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}