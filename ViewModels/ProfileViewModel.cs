using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class ProfileViewModel : BaseViewModel
{
    private readonly UserService _users;
    private readonly DatabaseService _db;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public ProfileViewModel(UserService users, DatabaseService db, NavigationService nav, DialogService dialog)
    {
        _users = users;
        _db = db;
        _nav = nav;
        _dialog = dialog;

        LogoutCommand = new Command(async () => await LogoutAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        PickImageCommand = new Command(async () => await PickImageAsync());
        RemoveImageCommand = new Command(() =>
        {
            ProfileImageUrl = string.Empty;
            OnPropertyChanged(nameof(ProfileImageUrl));
        });

        LoadFromCache();
        _ = LoadFromServerAsync();
    }

    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string Role { get; private set; } = string.Empty;

    public ICommand LogoutCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand PickImageCommand { get; }
    public ICommand RemoveImageCommand { get; }

    // Loads whatever's cached locally first, so the page shows something
    // instantly rather than a blank screen while the network call is in
    // flight.
    private void LoadFromCache()
    {
        var user = _db.LoadUser<User>();
        if (user == null) return;

        DisplayName = user.DisplayName;
        Email = user.Email;
        ProfileImageUrl = user.ProfileImageUrl ?? string.Empty;
        Role = user.Role;

        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(Email));
        OnPropertyChanged(nameof(ProfileImageUrl));
        OnPropertyChanged(nameof(Role));
    }

    // Then refresh from the server - the cache could be stale if the
    // profile was updated on another device, or by an Admin action like
    // deactivation (Role/IsActive could have changed).
    public async Task LoadFromServerAsync()
    {
        var user = await _users.GetMeAsync();
        if (user == null) return; // stay on cached values rather than blank the page on a network hiccup

        DisplayName = user.DisplayName;
        Email = user.Email;
        ProfileImageUrl = user.ProfileImageUrl ?? string.Empty;
        Role = user.Role;

        OnPropertyChanged(nameof(DisplayName));
        OnPropertyChanged(nameof(Email));
        OnPropertyChanged(nameof(ProfileImageUrl));
        OnPropertyChanged(nameof(Role));

        await _db.SaveUserAsync(user);
        SessionManager.CurrentUser = user;
    }

    private async Task PickImageAsync()
    {
        var base64 = await ImagePickerHelper.PickImageAsBase64Async();
        if (base64 == null) return;

        ProfileImageUrl = base64;
        OnPropertyChanged(nameof(ProfileImageUrl));
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(DisplayName) || string.IsNullOrWhiteSpace(Email))
        {
            await _dialog.ShowAlert("Error", "Display name and email are required.");
            return;
        }

        var dto = new UpdateProfileDto
        {
            DisplayName = DisplayName,
            Email = Email,
            ProfileImageUrl = string.IsNullOrWhiteSpace(ProfileImageUrl) ? null : ProfileImageUrl
        };

        var result = await _users.UpdateProfileAsync(dto);

        if (result == null)
        {
            await _dialog.ShowAlert("Error", "Couldn't update your profile." + (string.IsNullOrWhiteSpace(_users.LastError) ? "" : $"\n\n({_users.LastError})"));
            return;
        }

        await _db.SaveUserAsync(result);
        SessionManager.CurrentUser = result;

        await _dialog.ShowAlert("Success", "Profile updated!");
    }

    private async Task LogoutAsync()
    {
        _db.ClearToken();
        _db.ClearUser();
        SessionManager.Logout();

        // Removes the Admin tab if it was there - the fixed tabs
        // themselves aren't part of the flyout anymore, so there's nothing
        // else to tear down; the Navigating guard blocks access to them
        // the moment SessionManager reports logged-out anyway.
        var shell = Shell.Current as AppShell;
        shell?.RemoveAdminTab();

        await _nav.GoToAsync("//LoginPage");
    }
}
