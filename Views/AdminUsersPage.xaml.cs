using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminUsersPage : ContentPage
{
    public AdminUsersPage(AdminUsersViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminUsersViewModel vm)
            vm.RefreshCommand.Execute(null);
    }
}
