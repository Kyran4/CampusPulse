using System.Collections.ObjectModel;

namespace CampusPulse.ViewModels;

public class FeedViewModel
{
    public ObservableCollection<FeedPostItem> Posts { get; set; }

    public FeedViewModel()
    {
        Posts = new ObservableCollection<FeedPostItem>
        {
            new FeedPostItem
            {
                Title = "Campus Football Training",
                Category = "Sports",
                Content = "Football training is on this Friday after class.",
                Date = "Today"
            },

            new FeedPostItem
            {
                Title = "Gaming Club Meetup",
                Category = "Gaming",
                Content = "Come along to the gaming club meetup this week.",
                Date = "Today"
            },

            new FeedPostItem
            {
                Title = "Study Support Session",
                Category = "Study Support",
                Content = "Extra study support is available in the library.",
                Date = "Yesterday"
            }
        };
    }
}

public class FeedPostItem
{
    public string Title { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Date { get; set; } = string.Empty;
}