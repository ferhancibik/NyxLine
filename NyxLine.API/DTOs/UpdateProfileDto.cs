using System.ComponentModel.DataAnnotations;

namespace NyxLine.API.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Bio { get; set; }

        // Web'den gelen dosya
        public IFormFile? ProfileImage { get; set; }

        // MAUI'den gelen Base64 string
        public string? ProfileImageBase64 { get; set; }
    }
} 