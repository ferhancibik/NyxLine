using System.ComponentModel.DataAnnotations;
using NyxLine.API.Models;

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
        
        public PostType Type { get; set; } = PostType.Regular;
        
        [MaxLength(200, ErrorMessage = "Haber başlığı en fazla 200 karakter olabilir")]
        public string? NewsTitle { get; set; }
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
        public PostType Type { get; set; }
        public string? NewsTitle { get; set; }
        public bool IsUserAdmin { get; set; }

        // Repost özellikleri
        public bool IsRepost { get; set; }
        public int? OriginalPostId { get; set; }
        public PostResponseDto? OriginalPost { get; set; }
        public int RepostCount { get; set; }
        public bool IsRepostedByCurrentUser { get; set; }
    }

    public class UpdatePostDto
    {
        [Required(ErrorMessage = "İçerik zorunludur")]
        [MaxLength(2000, ErrorMessage = "İçerik en fazla 2000 karakter olabilir")]
        public string Content { get; set; } = string.Empty;
        
        [MaxLength(200, ErrorMessage = "Haber başlığı en fazla 200 karakter olabilir")]
        public string? NewsTitle { get; set; }
    }

    public class AdminBanUserDto
    {
        [Required(ErrorMessage = "Kullanıcı ID'si zorunludur")]
        public string UserId { get; set; } = string.Empty;
        
        [MaxLength(500, ErrorMessage = "Ban sebebi en fazla 500 karakter olabilir")]
        public string? Reason { get; set; }
    }

    public class AdminUnbanUserDto
    {
        [Required(ErrorMessage = "Kullanıcı ID'si zorunludur")]
        public string UserId { get; set; } = string.Empty;
    }
} 