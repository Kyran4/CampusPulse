using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminModerationPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public AdminModerationPage(AdminModerationViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminModerationViewModel vm)
            vm.RefreshCommand.Execute(null);

        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => (BindingContext as AdminModerationViewModel)?.RefreshCommand.Execute(null);
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
