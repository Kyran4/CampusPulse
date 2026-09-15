using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminHubPage : ContentPage
{
    public AdminHubPage(AdminHubViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
