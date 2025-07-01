using System.Text.Json.Serialization;

namespace NyxLine.MAUI.Models
{
    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("profileImagePath")]
        public string? ProfileImagePath { get; set; }

        [JsonPropertyName("isGhost")]
        public bool IsGhost { get; set; }

        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonPropertyName("postsCount")]
        public int PostsCount { get; set; }

        [JsonPropertyName("followersCount")]
        public int FollowersCount { get; set; }

        [JsonPropertyName("followingCount")]
        public int FollowingCount { get; set; }

        [JsonPropertyName("isFollowedByCurrentUser")]
        public bool IsFollowedByCurrentUser { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

            public string FullName => $"{FirstName} {LastName}";
    public string ProfileImageUrl => string.IsNullOrEmpty(ProfileImagePath) 
        ? "https://via.placeholder.com/100x100/cccccc/ffffff?text=👤" 
        : $"http://localhost:8080{ProfileImagePath}?v={Guid.NewGuid()}&t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&r={new Random().Next(1000, 9999)}";
    }
} 