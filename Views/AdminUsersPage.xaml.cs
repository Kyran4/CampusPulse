using CampusPulse.ViewModels;

namespace CampusPulse.Views;

public partial class AdminUsersPage : ContentPage
{
    private IDispatcherTimer _pollTimer;

    public AdminUsersPage(AdminUsersViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is AdminUsersViewModel vm)
            vm.RefreshCommand.Execute(null);

        // Other users registering/being deactivated by another Admin.
        _pollTimer = Application.Current.Dispatcher.CreateTimer();
        _pollTimer.Interval = TimeSpan.FromSeconds(7);
        _pollTimer.Tick += (s, e) => _ = (BindingContext as AdminUsersViewModel)?.PollAsync();
        _pollTimer.Start();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _pollTimer?.Stop();
        _pollTimer = null;
    }
}
