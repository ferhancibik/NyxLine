using System.Text.Json.Serialization;
using System.ComponentModel;

namespace NyxLine.MAUI.Models
{
    public enum PostType
    {
        Regular = 0,
        News = 1
    }

    public class Post : INotifyPropertyChanged
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

        private int _likeCount;
        [JsonPropertyName("likeCount")]
        public int LikeCount 
        { 
            get => _likeCount;
            set
            {
                if (_likeCount != value)
                {
                    _likeCount = value;
                    OnPropertyChanged(nameof(LikeCount));
                }
            }
        }

        private bool _isLikedByCurrentUser;
        [JsonPropertyName("isLikedByCurrentUser")]
        public bool IsLikedByCurrentUser 
        { 
            get => _isLikedByCurrentUser;
            set
            {
                if (_isLikedByCurrentUser != value)
                {
                    _isLikedByCurrentUser = value;
                    OnPropertyChanged(nameof(IsLikedByCurrentUser));
                }
            }
        }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("type")]
        public PostType Type { get; set; } = PostType.Regular;

        [JsonPropertyName("newsTitle")]
        public string? NewsTitle { get; set; }

        [JsonPropertyName("isUserAdmin")]
        public bool IsUserAdmin { get; set; }

        // Repost özellikleri
        private bool _isRepostedByCurrentUser;
        [JsonPropertyName("isRepostedByCurrentUser")]
        public bool IsRepostedByCurrentUser
        {
            get => _isRepostedByCurrentUser;
            set
            {
                if (_isRepostedByCurrentUser != value)
                {
                    _isRepostedByCurrentUser = value;
                    OnPropertyChanged(nameof(IsRepostedByCurrentUser));
                }
            }
        }

        [JsonPropertyName("isRepost")]
        public bool IsRepost { get; set; }

        [JsonPropertyName("originalPostId")]
        public int? OriginalPostId { get; set; }

        [JsonPropertyName("originalPost")]
        public Post? OriginalPost { get; set; }

        [JsonPropertyName("repostCount")]
        public int RepostCount { get; set; }

        public string PostImageUrl => string.IsNullOrEmpty(ImagePath) 
            ? string.Empty 
            : $"http://localhost:8080{ImagePath}?v={Guid.NewGuid()}&t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        public string UserProfileImageUrl => string.IsNullOrEmpty(UserProfileImage) 
            ? "https://via.placeholder.com/40x40/cccccc/ffffff?text=👤" 
            : $"http://localhost:8080{UserProfileImage}?v={Guid.NewGuid()}&t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        public bool HasImage => !string.IsNullOrEmpty(ImagePath);

        public bool HasUserProfileImage => !string.IsNullOrEmpty(UserProfileImage);

        public string LikeImageSource => IsLikedByCurrentUser 
            ? "heart_filled.png" 
            : "heart_outline.png";

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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 