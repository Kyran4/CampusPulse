using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class InterestsPage : ContentPage
{
    public InterestsPage(InterestsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
