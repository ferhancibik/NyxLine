using NyxLine.API.DTOs;
using NyxLine.API.Models;

namespace NyxLine.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, User? User)> RegisterAsync(UserRegistrationDto registrationDto);
        Task<(bool Success, string Message, string? Token, User? User)> LoginAsync(UserLoginDto loginDto);
        Task<(bool Success, string Message)> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<(bool Success, string Message, string? Token)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task LogoutAsync(string userId);
        string GenerateJwtToken(User user);
    }
} 