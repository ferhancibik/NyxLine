namespace NyxLine.API.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;

        public long MaxFileSize => 5 * 1024 * 1024; // 5MB
        public string[] AllowedExtensions => new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public async Task<string?> SaveFileAsync(IFormFile file, string folder)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                if (!IsValidImageFile(file))
                    return null;

                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                _logger.LogInformation($"Uploads path: {uploadsPath}");

                if (!Directory.Exists(uploadsPath))
                {
                    _logger.LogInformation($"Creating directory: {uploadsPath}");
                    Directory.CreateDirectory(uploadsPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);
                _logger.LogInformation($"File path: {filePath}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return $"/uploads/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file");
                return null;
            }
        }

        // YENİ: Base64 byte array'inden dosya kaydetme
        public async Task<string?> SaveFileFromBytesAsync(byte[] fileBytes, string fileName, string folder)
        {
            try
            {
                _logger.LogDebug("SaveFileFromBytesAsync başlatıldı");
                
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    _logger.LogError("fileBytes null veya boş");
                    return null;
                }

                if (fileBytes.Length > MaxFileSize)
                {
                    _logger.LogError($"Dosya boyutu çok büyük: {fileBytes.Length} > {MaxFileSize}");
                    return null;
                }

                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                _logger.LogInformation($"Uploads path: {uploadsPath}");

                // Klasör yoksa oluştur
                if (!Directory.Exists(uploadsPath))
                {
                    _logger.LogInformation($"Creating directory: {uploadsPath}");
                    Directory.CreateDirectory(uploadsPath);
                }

                // Güvenli dosya adı oluştur
                var extension = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(extension))
                    extension = ".jpg";

                var safeFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, safeFileName);
                _logger.LogInformation($"File path: {filePath}");

                // Dosyayı kaydet
                await File.WriteAllBytesAsync(filePath, fileBytes);
                _logger.LogInformation("Dosya başarıyla kaydedildi");

                return $"/uploads/{folder}/{safeFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file from bytes");
                return null;
            }
        }

        public void DeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return;

                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSize)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return false;

            // Check file signature (magic numbers)
            using var reader = new BinaryReader(file.OpenReadStream());
            var signatures = new Dictionary<string, List<byte[]>>
            {
                { ".jpg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
                { ".jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
                { ".png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
                { ".gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
                { ".bmp", new List<byte[]> { new byte[] { 0x42, 0x4D } } }
            };

            if (signatures.TryGetValue(extension, out var signatureList))
            {
                var headerBytes = reader.ReadBytes(signatureList.Max(s => s.Length));
                return signatureList.Any(signature => 
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }

            return false;
        }
    }
} 