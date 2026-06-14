namespace VehicleInspectionSystem.Web.Models
{
    public class ExpiringInspectionDto
    {
        public int Id { get; set; }

        public string PlateNumber { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public DateTime? NextInspectionDate { get; set; }

        public int DaysLeft { get; set; }

        public string ReminderType { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

    }
}