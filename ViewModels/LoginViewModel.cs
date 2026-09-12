using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly AuthenticationService _auth;
    private readonly DatabaseService _db;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public LoginViewModel(AuthenticationService auth, DatabaseService db, NavigationService nav, DialogService dialog)
    {
        _auth = auth;
        _db = db;
        _nav = nav;
        _dialog = dialog;

        LoginCommand = new Command(async () => await LoginAsync());
        GoToRegisterCommand = new Command(async () => await _nav.GoToAsync("RegisterPage"));
    }

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ICommand LoginCommand { get; }
    public ICommand GoToRegisterCommand { get; }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await _dialog.ShowAlert("Error", "Please enter both email and password.");
            return;
        }

        var result = await _auth.LoginAsync(new LoginRequest
        {
            Email = Email,
            Password = Password
        });

        if (result == null)
        {
            await _dialog.ShowAlert("Login Failed", "Invalid email or password.");
            return;
        }

        await _db.SaveUserAsync(result.User);
        await _db.SaveTokenAsync(result.Token);
        SessionManager.CurrentUser = result.User;

        var shell = Shell.Current as AppShell;

        shell?.RemoveAuthFlyout();
        shell?.RemoveCreatePostFlyout();
        shell?.RemoveAdminPages();

        if (!string.IsNullOrWhiteSpace(result.Token))
            shell?.AddCreatePostFlyout();

        if (SessionManager.IsAdmin)
            shell?.AddAdminPages();

        await _nav.GoToAsync("//FeedPage");
    }
}