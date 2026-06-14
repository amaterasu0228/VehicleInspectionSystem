namespace DocumentService.API.Models
{
    public class DocumentFile
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }
}