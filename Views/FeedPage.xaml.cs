using CampusPulse.Services;
using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class FeedPage : ContentPage
{
    public FeedPage(FeedViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var db = new DatabaseService();
        var token = await db.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }
    }
}
