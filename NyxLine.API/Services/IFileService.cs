namespace NyxLine.API.Services
{
    public interface IFileService
    {
        Task<string?> SaveFileAsync(IFormFile file, string folder);
        Task<string?> SaveFileFromBytesAsync(byte[] fileBytes, string fileName, string folder);
        Task<string?> SaveBase64ImageAsync(string base64String, string folder, string? fileName = null);
        void DeleteFile(string filePath);
        Task DeleteFileAsync(string filePath);
        bool IsValidImageFile(IFormFile file);
        long MaxFileSize { get; }
        string[] AllowedExtensions { get; }
    }
} 