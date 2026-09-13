using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProfileViewModel vm)
            _ = vm.LoadFromServerAsync();
    }
}
