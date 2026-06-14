using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleInspectionSystem.Web.Services;

namespace VehicleInspectionSystem.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class DashboardController : Controller
    {
        private readonly ApiService _api;

        public DashboardController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index(
    DateTime? dateFrom,
    DateTime? dateTo)
        {
            var model = await _api.GetDashboardAsync(
                dateFrom,
                dateTo);

            var appointmentStats = await _api.GetAppointmentStatisticsAsync();

            var activeReminders = await _api.GetActiveRemindersAsync();
            var readReminders = await _api.GetReadRemindersAsync();

            ViewBag.ActiveReminders = activeReminders;
            ViewBag.ReadReminders = readReminders;

            ViewBag.AppointmentStats = appointmentStats;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MarkReminderAsRead(int id)
        {
            await _api.MarkReminderAsReadAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> MarkReminderAsUnread(int id)
        {
            await _api.MarkReminderAsUnreadAsync(id);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> ExportPdf(DateTime? dateFrom, DateTime? dateTo)
        {
            var dashboard = await _api.GetDashboardAsync(dateFrom, dateTo);
            var appointmentStats = await _api.GetAppointmentStatisticsAsync();
            var activeReminders = await _api.GetActiveRemindersAsync();
            var readReminders = await _api.GetReadRemindersAsync();

            if (dashboard == null)
                return RedirectToAction(nameof(Index));

            var report = new VehicleInspectionSystem.Web.Models.AnalyticsReportDto
            {
                DateFrom = dashboard.DateFrom ?? "-",
                DateTo = dashboard.DateTo ?? "-",

                TotalInspections = dashboard.TotalInspections,
                InternationalInspections = dashboard.InternationalInspections,
                NonComplianceActs = dashboard.NonComplianceActs,
                CanceledCount = dashboard.CanceledCount,

                ClientsCount = dashboard.ClientsCount,
                NewClientsCount = dashboard.NewClientsCount,

                TotalAppointments = appointmentStats?.Total ?? 0,
                NewAppointments = appointmentStats?.NewCount ?? 0,
                ConfirmedAppointments = appointmentStats?.ConfirmedCount ?? 0,
                CompletedAppointments = appointmentStats?.CompletedCount ?? 0,

                ActiveReminders = activeReminders.Count,
                ReadReminders = readReminders.Count,

                GeneratedBy = User.Identity?.Name ?? "System",

                VehicleTypeStats = dashboard.ByVehicleType
        .Select(x => new VehicleInspectionSystem.Web.Models.ReportChartItemDto
        {
            Name = x.Name ?? "-",
            Count = x.Count
        })
        .ToList(),

                CategoryStats = dashboard.ByCategory
        .Select(x => new VehicleInspectionSystem.Web.Models.ReportChartItemDto
        {
            Name = x.Name ?? "-",
            Count = x.Count
        })
        .ToList(),

                DateStats = dashboard.ByDate
        .Select(x => new VehicleInspectionSystem.Web.Models.ReportDateChartItemDto
        {
            Date = x.Date ?? "-",
            Count = x.Count
        })
        .ToList()
            };

            var pdfBytes = await _api.GenerateAnalyticsPdfAsync(report);

            if (pdfBytes.Length == 0)
                return RedirectToAction(nameof(Index));

            return File(
                pdfBytes,
                "application/pdf",
                $"analytics-report-{DateTime.Now:yyyyMMdd-HHmm}.pdf"
            );



        }

    }
}