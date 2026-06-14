namespace VehicleInspectionSystem.Web.Models
{
    public class DocumentDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}