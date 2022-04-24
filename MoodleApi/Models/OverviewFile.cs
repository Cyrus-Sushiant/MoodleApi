namespace MoodleApi.Models
{
    public class OverviewFile
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public int FileSize { get; set; }
        public string? FileUrl { get; set; }
        public int TimeModified { get; set; }
        public string? MimeType { get; set; }
    }
}
