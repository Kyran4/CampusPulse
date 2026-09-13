using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class MyPostsPage : ContentPage
{
    public MyPostsPage(MyPostsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Same reasoning as FeedPage - this page is constructed once and
        // reused, so without this, deleting/creating a post and coming
        // back here would still show the old list.
        if (BindingContext is MyPostsViewModel vm)
            vm.RefreshCommand.Execute(null);
    }
}
