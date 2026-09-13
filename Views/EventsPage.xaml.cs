using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class EventsPage : ContentPage
{
    public EventsPage(EventsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EventsViewModel vm)
            vm.RefreshCommand.Execute(null);
    }
}
