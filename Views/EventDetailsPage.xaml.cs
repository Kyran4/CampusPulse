using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class EventDetailsPage : ContentPage
{
    public EventDetailsPage(EventDetailsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
