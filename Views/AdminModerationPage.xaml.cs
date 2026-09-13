using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminModerationPage : ContentPage
{
    public AdminModerationPage(AdminModerationViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminModerationViewModel vm)
            vm.RefreshCommand.Execute(null);
    }
}
