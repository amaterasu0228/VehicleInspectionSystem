namespace VehicleInspectionSystem.Web.Models
{
    public class DashboardDto
    {
        public string? DateFrom { get; set; }
        public string? DateTo { get; set; }

        public int TotalInspections { get; set; }
        public int ComplianceProtocols { get; set; }
        public int NonComplianceActs { get; set; }
        public int InternationalInspections { get; set; }

        public int ClientsCount { get; set; }
        public int NewClientsCount { get; set; }

        public int SubjectToInspectionCount { get; set; }
        public int CanceledCount { get; set; }

        public List<ChartItemDto> ByVehicleType { get; set; } = new();
        public List<ChartItemDto> ByCategory { get; set; } = new();
        public List<ChartItemDto> ByStatus { get; set; } = new();
        public List<ChartItemDto> ByOwnerType { get; set; } = new();
        public List<DateChartItemDto> ByDate { get; set; } = new();
    }

    public class ChartItemDto
    {
        public string? Name { get; set; }
        public int Count { get; set; }
    }

    public class DateChartItemDto
    {
        public string? Date { get; set; }
        public int Count { get; set; }
    }
}