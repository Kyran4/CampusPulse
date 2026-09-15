using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

public class AdminModerationViewModel : BaseViewModel
{
    private readonly ReportService _reports;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public AdminModerationViewModel(ReportService reports, NavigationService nav, DialogService dialog)
    {
        _reports = reports;
        _nav = nav;
        _dialog = dialog;

        Reports = new ObservableCollection<Report>();
        RefreshCommand = new Command(async () => await LoadReportsAsync());
        OpenPostCommand = new Command<int?>(async (id) => await OpenPostAsync(id));
        DismissCommand = new Command<Report>(async (r) => await ReviewAsync(r, "Dismissed"));
        ActionTakenCommand = new Command<Report>(async (r) => await ReviewAsync(r, "ActionTaken"));

        _ = LoadReportsAsync();
    }

    // Only Pending reports need attention here - anything already reviewed
    // belongs in a history view rather than cluttering the queue.
    public ObservableCollection<Report> Reports { get; }

    public ICommand RefreshCommand { get; }
    public ICommand OpenPostCommand { get; }
    public ICommand DismissCommand { get; }
    public ICommand ActionTakenCommand { get; }

    private async Task LoadReportsAsync(bool showErrorAlert = true)
    {
        IsBusy = true;

        var list = await _reports.GetReportsAsync("Pending");
        Reports.Clear();

        if (list != null)
        {
            foreach (var r in list)
                Reports.Add(r);
        }
        else if (showErrorAlert)
        {
            await _dialog.ShowAlert("Error", "Couldn't load reports." + (string.IsNullOrWhiteSpace(_reports.LastError) ? "" : $"\n\n({_reports.LastError})"));
        }

        IsBusy = false;
    }

    public async Task PollAsync() => await LoadReportsAsync(showErrorAlert: false);

    public async Task OpenPostAsync(int? postId)
    {
        if (postId.HasValue)
            await _nav.GoToAsync($"PostDetailsPage?postId={postId.Value}");
    }

    private async Task ReviewAsync(Report report, string status)
    {
        if (report == null) return;

        var result = await _reports.ReviewReportAsync(report.ReportId, new ReportReviewDto { Status = status });

        if (result != null)
        {
            Reports.Remove(report);
        }
        else
        {
            await _dialog.ShowAlert("Error", "Could not update this report.");
        }
    }
}
