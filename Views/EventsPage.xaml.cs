using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class EventsPage : ContentPage
{
    public EventsPage(EventsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
