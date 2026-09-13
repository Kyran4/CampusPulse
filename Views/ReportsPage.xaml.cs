using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class ReportsPage : ContentPage
{
    public ReportsPage(ReportsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ReportsViewModel vm)
            vm.RefreshCommand.Execute(null);
    }
}
