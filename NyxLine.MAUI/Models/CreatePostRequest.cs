namespace NyxLine.MAUI.Models
{
    public class CreatePostRequest
    {
        public string Content { get; set; } = string.Empty;
        public string? ImageBase64 { get; set; }
        public string? FileName { get; set; }
    }
} 