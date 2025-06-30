namespace NyxLine.API.Services
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(IFormFile file, string folder);
        Task<string?> SaveFileFromBytesAsync(byte[] fileBytes, string fileName, string folder);
        void DeleteFile(string filePath);
        bool IsValidImageFile(IFormFile file);
        long MaxFileSize { get; }
        string[] AllowedExtensions { get; }
    }
} 