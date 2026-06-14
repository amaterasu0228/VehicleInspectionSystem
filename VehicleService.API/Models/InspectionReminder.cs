namespace VehicleService.API.Models
{
    public class InspectionReminder
    {
        public int Id { get; set; }

        public int InspectionId { get; set; }

        public string ReminderType { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}