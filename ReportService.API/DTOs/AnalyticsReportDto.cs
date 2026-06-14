namespace ReportService.API.DTOs
{
    public class AnalyticsReportDto
    {
        public string DateFrom { get; set; } = string.Empty;
        public string DateTo { get; set; } = string.Empty;

        public int TotalInspections { get; set; }
        public int InternationalInspections { get; set; }
        public int NonComplianceActs { get; set; }
        public int CanceledCount { get; set; }

        public int ClientsCount { get; set; }
        public int NewClientsCount { get; set; }

        public int TotalAppointments { get; set; }
        public int NewAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CompletedAppointments { get; set; }

        public int ActiveReminders { get; set; }
        public int ReadReminders { get; set; }

        public string GeneratedBy { get; set; } = string.Empty;

        public List<ReportChartItemDto> VehicleTypeStats { get; set; } = new();
        public List<ReportChartItemDto> CategoryStats { get; set; } = new();
    }
    public class ReportChartItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class ReportDateChartItemDto
    {
        public string Date { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}