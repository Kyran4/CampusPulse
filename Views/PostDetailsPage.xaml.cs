using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class PostDetailsPage : ContentPage
{
    public PostDetailsPage(PostDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
