using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class CreatePostPage : ContentPage
{
    public CreatePostPage(CreatePostViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (BindingContext as CreatePostViewModel).LoadCategoriesAsync();
    }
}
