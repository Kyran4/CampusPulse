using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly DatabaseService _db;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public ProfileViewModel(DatabaseService db, NavigationService nav, DialogService dialog)
    {
        _db = db;
        _nav = nav;
        _dialog = dialog;

        LogoutCommand = new Command(async () => await LogoutAsync());
        LoadProfile();
    }

    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICommand LogoutCommand { get; }

    private void LoadProfile()
    {
        var user = _db.LoadUser<User>();
        if (user != null)
        {
            DisplayName = user.DisplayName;
            Email = user.Email;
        }
    }

    private async Task LogoutAsync()
    {
        _db.ClearToken();
        _db.ClearUser();

        await _nav.GoToAsync("//LoginPage");
    }
}
