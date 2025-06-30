using NyxLine.MAUI.Models;

namespace NyxLine.MAUI.Services
{
    public interface IAuthService
    {
        Task<bool> IsLoggedInAsync();
        Task<User?> GetCurrentUserAsync();
        Task<User?> RefreshCurrentUserAsync();
        void ClearUserCache();
        Task<bool> LoginAsync(string emailOrUsername, string password);
        Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
        Task<string?> GetTokenAsync();
        event EventHandler<bool> AuthStateChanged;
    }
} 