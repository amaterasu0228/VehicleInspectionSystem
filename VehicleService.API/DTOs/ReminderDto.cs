namespace VehicleService.API.DTOs
{
    public class ReminderDto
    {
        public int Id { get; set; }

        public int InspectionId { get; set; }

        public string ReminderType { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public string DocumentNumber { get; set; } = string.Empty;

        public string VehicleType { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string PlateNumber { get; set; } = string.Empty;

        public string VIN { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public string OwnerPhone { get; set; } = string.Empty;

        public DateTime? NextInspectionDate { get; set; }

        public int DaysLeft { get; set; }
    }
}