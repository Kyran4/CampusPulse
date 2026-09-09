using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class InterestsPage : ContentPage
{
    public InterestsPage()
    {
        InitializeComponent();

        BindingContext = new InterestsViewModel();
    }
}