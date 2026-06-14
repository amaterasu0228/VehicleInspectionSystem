namespace VehicleInspectionSystem.Web.Models
{
    public class AppointmentStatisticsDto
    {
        public int Total { get; set; }

        public int NewCount { get; set; }

        public int ConfirmedCount { get; set; }

        public int CompletedCount { get; set; }

        public int RejectedCount { get; set; }
    }
}