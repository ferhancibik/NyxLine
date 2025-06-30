using System.Text.Json.Serialization;

namespace NyxLine.MAUI.Models
{
    public class UpdateProfileRequest
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("profileImageBase64")]
        public string? ProfileImageBase64 { get; set; }
    }
} 