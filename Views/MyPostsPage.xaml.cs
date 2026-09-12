using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class MyPostsPage : ContentPage
{
    public MyPostsPage(MyPostsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
