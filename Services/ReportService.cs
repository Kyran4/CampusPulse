using CampusPulse.DTOs;
using CampusPulse.Helpers;
using CampusPulse.Models;
using SQLite;

namespace CampusPulse.Services;

public class ReportService
{
    private readonly SQLiteAsyncConnection _db;

    public ReportService(DatabaseService database)
    {
        _db = database.Connection;
    }

    public async Task<bool> CreateReportAsync(ReportCreateDto dto)
    {
        var user = SessionManager.CurrentUser;
        if (user == null) return false;

        var report = new Report
        {
            ReportedByUserId = user.UserId,
            PostId = dto.PostId,
            CommentId = dto.CommentId,
            Reason = dto.Reason
        };

        await _db.InsertAsync(report);
        return true;
    }

    public Task<List<Report>> GetPendingReportsAsync()
    {
        return _db.Table<Report>()
            .Where(r => r.Status == "Pending")
            .OrderBy(r => r.CreatedDate)
            .ToListAsync();
    }

    public async Task<bool> UpdateReportStatusAsync(int reportId, string status)
    {
        var admin = SessionManager.CurrentUser;
        if (admin == null || admin.Role != "Admin")
            return false;

        var report = await _db.Table<Report>()
            .Where(r => r.ReportId == reportId)
            .FirstOrDefaultAsync();

        if (report == null)
            return false;

        report.Status = status;
        report.ReviewedBy = admin.UserId;

        await _db.UpdateAsync(report);
        return true;
    }
}
