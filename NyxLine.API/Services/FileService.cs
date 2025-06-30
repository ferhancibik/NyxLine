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
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

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
                Console.WriteLine($"[FileService DEBUG] SaveFileFromBytesAsync başlatıldı");
                Console.WriteLine($"[FileService DEBUG] fileBytes null mu: {fileBytes == null}");
                
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    Console.WriteLine($"[FileService ERROR] fileBytes null veya boş");
                    return null;
                }

                Console.WriteLine($"[FileService DEBUG] fileBytes uzunluğu: {fileBytes.Length}");
                Console.WriteLine($"[FileService DEBUG] MaxFileSize: {MaxFileSize}");

                // Dosya boyutu kontrolü
                if (fileBytes.Length > MaxFileSize)
                {
                    Console.WriteLine($"[FileService ERROR] Dosya boyutu çok büyük: {fileBytes.Length} > {MaxFileSize}");
                    return null;
                }

                Console.WriteLine($"[FileService DEBUG] WebRootPath: {_environment.WebRootPath}");
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                Console.WriteLine($"[FileService DEBUG] uploadsPath: {uploadsPath}");
                
                if (!Directory.Exists(uploadsPath))
                {
                    Console.WriteLine($"[FileService DEBUG] Klasör yok, oluşturuluyor: {uploadsPath}");
                    Directory.CreateDirectory(uploadsPath);
                }
                else
                {
                    Console.WriteLine($"[FileService DEBUG] Klasör mevcut: {uploadsPath}");
                }

                // Güvenli dosya adı oluştur
                var extension = Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(extension))
                    extension = ".jpg"; // Varsayılan uzantı

                var safeFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, safeFileName);
                Console.WriteLine($"[FileService DEBUG] Tam dosya yolu: {filePath}");

                // Dosyayı kaydet
                await File.WriteAllBytesAsync(filePath, fileBytes);
                Console.WriteLine($"[FileService DEBUG] Dosya başarıyla kaydedildi");

                var returnPath = $"/uploads/{folder}/{safeFileName}";
                Console.WriteLine($"[FileService DEBUG] Return path: {returnPath}");
                
                return returnPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FileService ERROR] Hata: {ex.Message}");
                Console.WriteLine($"[FileService ERROR] Stack trace: {ex.StackTrace}");
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