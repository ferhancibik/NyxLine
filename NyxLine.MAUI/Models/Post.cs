using System.Text.Json.Serialization;

namespace NyxLine.MAUI.Models
{
    public class Post
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("imagePath")]
        public string? ImagePath { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("userFullName")]
        public string UserFullName { get; set; } = string.Empty;

        [JsonPropertyName("userProfileImage")]
        public string? UserProfileImage { get; set; }

        [JsonPropertyName("likeCount")]
        public int LikeCount { get; set; }

        [JsonPropertyName("isLikedByCurrentUser")]
        public bool IsLikedByCurrentUser { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        public string PostImageUrl => string.IsNullOrEmpty(ImagePath) 
            ? string.Empty 
            : $"http://localhost:8080{ImagePath}?v={Guid.NewGuid()}&t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        public string UserProfileImageUrl => string.IsNullOrEmpty(UserProfileImage) 
            ? "https://via.placeholder.com/40x40/cccccc/ffffff?text=👤" 
            : $"http://localhost:8080{UserProfileImage}?v={Guid.NewGuid()}&t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedAt;
                if (timeSpan.TotalMinutes < 1) return "şimdi";
                if (timeSpan.TotalHours < 1) return $"{(int)timeSpan.TotalMinutes}dk önce";
                if (timeSpan.TotalDays < 1) return $"{(int)timeSpan.TotalHours}sa önce";
                if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays}g önce";
                if (timeSpan.TotalDays < 30) return $"{(int)(timeSpan.TotalDays / 7)}h önce";
                return CreatedAt.ToString("dd.MM.yyyy");
            }
        }
    }
} 