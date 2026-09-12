using CampusPulse.Helpers;
using CampusPulse.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AuthenticationService _authenticationService;

    private string _email = string.Empty;
    private string _password = string.Empty;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        _authenticationService = new AuthenticationService();

        LoginCommand = new Command(
            async () => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert(
                "Login",
                "Please enter your email and password.",
                "OK");

            return;
        }

        var user = await _authenticationService
            .LoginAsync(Email, Password);

        if (user == null)
        {
            await Shell.Current.DisplayAlert(
                "Login Failed",
                "Email or password is incorrect.",
                "OK");

            return;
        }

        SessionManager.CurrentUser = user;

        await Shell.Current.DisplayAlert(
            "Login",
            $"Welcome {user.DisplayName}",
            "OK");

        await Shell.Current.GoToAsync("//Home");
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}