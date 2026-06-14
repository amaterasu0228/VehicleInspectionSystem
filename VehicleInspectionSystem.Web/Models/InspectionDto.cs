namespace VehicleInspectionSystem.Web.Models
{
    public class InspectionDto
    {
        public int Id { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;
        public string BlankNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public DateTime SignDate { get; set; }
        public TimeSpan SignTime { get; set; }

        public string VehicleCompliance { get; set; } = string.Empty;

        public string VehicleType { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string OwnerPhone { get; set; } = string.Empty;
        public string OwnerType { get; set; } = string.Empty;

        public bool IsSubjectToInspection { get; set; }
        public string InspectionPeriod { get; set; } = string.Empty;

        public DateTime? NextInspectionDate { get; set; }

        public bool IsInternational { get; set; }
        public bool IsCanceled { get; set; }
    }
}