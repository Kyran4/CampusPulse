using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminUsersPage : ContentPage
{
    public AdminUsersPage(AdminUsersViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
