using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminModerationPage : ContentPage
{
    public AdminModerationPage(AdminModerationViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
