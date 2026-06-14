namespace VehicleInspectionSystem.Web.Models
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string VehicleBrand { get; set; } = string.Empty;
        public string VehicleModel { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; } = "Новий";

        public DateTime CreatedAt { get; set; }
    }
}