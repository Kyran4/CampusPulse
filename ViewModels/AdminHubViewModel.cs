using CampusPulse.Services;
using MvvmHelpers;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

// Consolidates what used to be 4 separate flyout entries (Dashboard,
// Moderation, Reports, Manage Users) behind a single "Admin" tab, so the
// bottom bar stays a clean, fixed 5-6 items instead of growing per admin
// feature. Each button below just navigates to the same pages as before.
public class AdminHubViewModel : BaseViewModel
{
    private readonly NavigationService _nav;

    public AdminHubViewModel(NavigationService nav)
    {
        _nav = nav;

        OpenDashboardCommand = new Command(async () => await _nav.GoToAsync("AdminDashboardPage"));
        OpenModerationCommand = new Command(async () => await _nav.GoToAsync("AdminModerationPage"));
        OpenReportsCommand = new Command(async () => await _nav.GoToAsync("ReportsPage"));
        OpenUsersCommand = new Command(async () => await _nav.GoToAsync("AdminUsersPage"));
    }

    public ICommand OpenDashboardCommand { get; }
    public ICommand OpenModerationCommand { get; }
    public ICommand OpenReportsCommand { get; }
    public ICommand OpenUsersCommand { get; }
}
