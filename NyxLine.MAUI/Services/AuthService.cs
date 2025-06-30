using NyxLine.MAUI.Models;

namespace NyxLine.MAUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;
        private readonly ISecureStorage _secureStorage;
        private User? _currentUser;

        public event EventHandler<bool>? AuthStateChanged;

        public AuthService(IApiService apiService, ISecureStorage secureStorage)
        {
            _apiService = apiService;
            _secureStorage = secureStorage;
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            if (_currentUser == null && await IsLoggedInAsync())
            {
                _currentUser = await _apiService.GetCurrentUserProfileAsync();
                System.Diagnostics.Debug.WriteLine($"AuthService: Current user loaded - {_currentUser?.UserName}");
            }
            else if (_currentUser == null)
            {
                System.Diagnostics.Debug.WriteLine("AuthService: User not logged in or token missing");
            }
            return _currentUser;
        }

        public async Task<User?> RefreshCurrentUserAsync()
        {
            if (await IsLoggedInAsync())
            {
                _currentUser = await _apiService.GetCurrentUserProfileAsync();
                System.Diagnostics.Debug.WriteLine($"AuthService: Current user refreshed - {_currentUser?.UserName}");
                return _currentUser;
            }
            return null;
        }

        public void ClearUserCache()
        {
            _currentUser = null;
            System.Diagnostics.Debug.WriteLine("AuthService: User cache cleared");
        }

        public async Task<bool> LoginAsync(string emailOrUsername, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    EmailOrUserName = emailOrUsername,
                    Password = password
                };

                var response = await _apiService.LoginAsync(loginRequest);
                if (response?.User != null)
                {
                    _currentUser = response.User;
                    AuthStateChanged?.Invoke(this, true);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("AuthService: RegisterAsync started");
                var response = await _apiService.RegisterAsync(request);
                
                System.Diagnostics.Debug.WriteLine($"AuthService: API Response received - {response?.Message}");
                
                if (response != null && !string.IsNullOrEmpty(response.Message))
                {
                    // API'den success mesajı geldi
                    if (response.Message.Contains("başarılı") || response.Message.Contains("tamamlandı"))
                    {
                        return (true, response.Message);
                    }
                    else
                    {
                        // API'den hata mesajı geldi
                        return (false, response.Message);
                    }
                }
                return (false, "API'den yanıt alınamadı veya boş yanıt geldi");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AuthService: Exception - {ex.Message}");
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _apiService.LogoutAsync();
            }
            catch
            {
                // Continue with logout even if API call fails
            }
            finally
            {
                _currentUser = null;
                AuthStateChanged?.Invoke(this, false);
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            var token = await _secureStorage.GetAsync("auth_token");
            System.Diagnostics.Debug.WriteLine($"AuthService: GetTokenAsync - Token length: {token?.Length ?? 0}");
            System.Diagnostics.Debug.WriteLine($"AuthService: GetTokenAsync - Token exists: {!string.IsNullOrEmpty(token)}");
            return token;
        }
    }
} 