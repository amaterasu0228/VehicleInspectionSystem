namespace VehicleService.API.Models
{
    public class VehicleInspection
    {
        public int Id { get; set; }

        public string DocumentNumber { get; set; }

        public string VehicleCompliance { get; set; } = string.Empty;

        public string BlankNumber { get; set; }
        public string Status { get; set; }

        public DateTime SignDate { get; set; }
        public TimeSpan SignTime { get; set; }

        

        public string VehicleType { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Category { get; set; }

        public string PlateNumber { get; set; }
        public string VIN { get; set; }

        public string Owner { get; set; }

        public string OwnerPhone { get; set; } = string.Empty;

        public string OwnerType { get; set; }

        public bool IsSubjectToInspection { get; set; }
        public string InspectionPeriod { get; set; }

        public DateTime? NextInspectionDate { get; set; }

        public bool IsInternational { get; set; }
        public bool IsCanceled { get; set; }
    }
}