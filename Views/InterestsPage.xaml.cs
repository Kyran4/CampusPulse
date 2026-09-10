using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class InterestsPage : ContentPage
{
    private readonly InterestsViewModel _viewModel;

    public InterestsPage()
    {
        InitializeComponent();

        _viewModel = new InterestsViewModel();
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadInterestsAsync();
    }
}