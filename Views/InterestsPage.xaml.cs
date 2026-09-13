using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class InterestsPage : ContentPage
{
    public InterestsPage(InterestsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is InterestsViewModel vm)
            vm.RefreshCommand.Execute(null);
    }
}
