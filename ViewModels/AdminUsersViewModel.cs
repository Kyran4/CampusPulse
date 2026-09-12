using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class AdminUsersViewModel : BaseViewModel
{
    private readonly AdminService _admin;
    private readonly DialogService _dialog;

    public AdminUsersViewModel(AdminService admin, DialogService dialog)
    {
        _admin = admin;
        _dialog = dialog;

        Users = new ObservableCollection<UserAdminDto>();

        RefreshCommand = new Command(async () => await LoadAsync());
        SearchCommand = new Command(async () => await LoadAsync());
        DeactivateCommand = new Command<UserAdminDto>(async (u) => await DeactivateAsync(u));
        ReactivateCommand = new Command<UserAdminDto>(async (u) => await ReactivateAsync(u));

        _ = LoadAsync();
    }

    public ObservableCollection<UserAdminDto> Users { get; }

    public string SearchText { get; set; } = string.Empty;

    public ICommand RefreshCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand DeactivateCommand { get; }
    public ICommand ReactivateCommand { get; }

    private async Task LoadAsync()
    {
        IsBusy = true;

        var list = await _admin.GetUsersAsync(SearchText);
        Users.Clear();

        if (list != null)
        {
            foreach (var u in list)
                Users.Add(u);
        }
        else
        {
            await _dialog.ShowAlert("Error", "Couldn't load users." + (string.IsNullOrWhiteSpace(_admin.LastError) ? "" : $"\n\n({_admin.LastError})"));
        }

        IsBusy = false;
    }

    private async Task DeactivateAsync(UserAdminDto user)
    {
        if (user == null) return;

        var confirmed = await _dialog.ShowConfirm("Deactivate User", $"Deactivate {user.DisplayName}? They won't be able to log in until reactivated.");
        if (!confirmed) return;

        if (await _admin.DeactivateUserAsync(user.UserId))
        {
            user.IsActive = false;
            await LoadAsync();
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not deactivate this user.");
        }
    }

    private async Task ReactivateAsync(UserAdminDto user)
    {
        if (user == null) return;

        if (await _admin.ReactivateUserAsync(user.UserId))
        {
            user.IsActive = true;
            await LoadAsync();
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not reactivate this user.");
        }
    }
}
