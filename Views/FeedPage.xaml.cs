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

        // FeedPage/FeedViewModel are constructed once and reused for the
        // life of the app (Shell doesn't recreate flyout pages on every
        // visit), so the constructor's initial load only ever ran once.
        // Without this, creating a post and navigating back showed the
        // feed exactly as it was before you posted - refreshing here means
        // every time you actually land on this page, it's current.
        if (BindingContext is FeedViewModel vm)
            vm.RefreshCommand.Execute(null);
    }
}
