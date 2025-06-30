using System.ComponentModel.DataAnnotations;

namespace NyxLine.API.DTOs
{
    public class CreatePostDto
    {
        [Required(ErrorMessage = "İçerik zorunludur")]
        [MaxLength(2000, ErrorMessage = "İçerik en fazla 2000 karakter olabilir")]
        public string Content { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        // MAUI'den base64 string olarak gönderilen resim için
        public string? ImageBase64 { get; set; }
        public string? FileName { get; set; }
    }

    public class PostResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdatePostDto
    {
        [Required(ErrorMessage = "İçerik zorunludur")]
        [MaxLength(2000, ErrorMessage = "İçerik en fazla 2000 karakter olabilir")]
        public string Content { get; set; } = string.Empty;
    }
} 