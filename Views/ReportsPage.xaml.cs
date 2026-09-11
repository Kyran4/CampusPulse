using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class ReportsPage : ContentPage
{
    public ReportsPage(ReportsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
