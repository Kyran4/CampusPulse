using CampusPulse.Models;

namespace CampusPulse.Services;

public class ReportService
{
    private readonly ApiClient _api;

    public string? LastError => _api.LastError;

    public ReportService(ApiClient api)
    {
        _api = api;
    }

    // Admin-only on the API side - see ReportsController.
    public async Task<List<Report>?> GetReportsAsync(string? status = null)
    {
        var url = string.IsNullOrWhiteSpace(status) ? "api/reports" : $"api/reports?status={status}";
        return await _api.GetAsync<List<Report>>(url);
    }

    public async Task<Report?> CreateReportAsync(ReportCreateDto dto)
    {
        return await _api.PostAsync<Report>("api/reports", dto);
    }

    public async Task<Report?> ReviewReportAsync(int id, ReportReviewDto dto)
    {
        return await _api.PutAsync<Report>($"api/reports/{id}/review", dto);
    }
}
