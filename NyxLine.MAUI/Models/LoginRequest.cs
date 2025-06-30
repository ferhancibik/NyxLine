using System.Text.Json.Serialization;

namespace NyxLine.MAUI.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("emailOrUserName")]
        public string EmailOrUserName { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("bio")]
        public string? Bio { get; set; }

        [JsonPropertyName("isGhost")]
        public bool IsGhost { get; set; }
    }

    public class ChangePasswordRequest
    {
        [JsonPropertyName("currentPassword")]
        public string CurrentPassword { get; set; } = string.Empty;

        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; } = string.Empty;

        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; } = string.Empty;

        [JsonPropertyName("confirmPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UpdatePostRequest
    {
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;
    }
} 