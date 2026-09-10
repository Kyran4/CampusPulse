using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class FeedPage : ContentPage
{
    public FeedPage()
    {
        InitializeComponent();

        BindingContext = new FeedViewModel();
    }
}