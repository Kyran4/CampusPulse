using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly AuthenticationService _auth;
    private readonly DatabaseService _db;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public RegisterViewModel(AuthenticationService auth, DatabaseService db, NavigationService nav, DialogService dialog)
    {
        _auth = auth;
        _db = db;
        _nav = nav;
        _dialog = dialog;

        RegisterCommand = new Command(async () => await RegisterAsync());
        GoToLoginCommand = new Command(async () => await _nav.GoToAsync("LoginPage"));
    }

    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ICommand RegisterCommand { get; }
    public ICommand GoToLoginCommand { get; }

    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(DisplayName) ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await _dialog.ShowAlert("Error", "All fields are required.");
            return;
        }

        if (!ValidationHelper.IsValidEmail(Email))
        {
            await _dialog.ShowAlert("Error", "Please enter a valid email address.");
            return;
        }

        if (!ValidationHelper.IsValidPassword(Password))
        {
            await _dialog.ShowAlert("Error", "Password must be at least 8 characters and include an uppercase letter, a lowercase letter, a digit and a symbol.");
            return;
        }

        var result = await _auth.RegisterAsync(new RegisterRequest
        {
            DisplayName = DisplayName,
            Email = Email,
            Password = Password
        });

        if (result == null)
        {
            await _dialog.ShowAlert("Registration Failed", "Could not create account. The email may already be in use.");
            return;
        }

        await _db.SaveUserAsync(result.User);
        await _db.SaveTokenAsync(result.Token);
        SessionManager.CurrentUser = result.User;

        var shell = Shell.Current as AppShell;

        shell?.RemoveCreatePostFlyout();
        shell?.RemoveAdminPages(); // new accounts are always Student

        if (!string.IsNullOrWhiteSpace(result.Token))
            shell?.AddCreatePostFlyout();

        await _nav.GoToAsync("//FeedPage");
    }
}
