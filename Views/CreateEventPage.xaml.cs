using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class CreateEventPage : ContentPage
{
    public CreateEventPage(CreateEventViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await (BindingContext as CreateEventViewModel).LoadCategoriesAsync();
    }
}
