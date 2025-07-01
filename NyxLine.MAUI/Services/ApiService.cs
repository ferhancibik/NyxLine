using NyxLine.MAUI.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NyxLine.MAUI.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ISecureStorage _secureStorage;
        private readonly string _baseUrl = "http://localhost:8080/api";

        public ApiService(HttpClient httpClient, ISecureStorage secureStorage)
        {
            _secureStorage = secureStorage;

            #if WINDOWS
            // Windows'ta localhost bağlantısı için
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            _httpClient = new HttpClient(handler);
            #else
            _httpClient = httpClient;
            #endif

            // HTTP istemcisinin yapılandırması
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async Task<string?> GetTokenAsync()
        {
            try 
            {
                return await _secureStorage.GetAsync("auth_token");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Token] Hata: {ex.Message}");
                return null;
            }
        }

        private async Task SetTokenAsync(string token)
        {
            try 
            {
                await _secureStorage.SetAsync("auth_token", token);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Token] Kaydetme hatası: {ex.Message}");
            }
        }

        private void ClearTokenAsync()
        {
            try 
            {
                _secureStorage.Remove("auth_token");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Token] Silme hatası: {ex.Message}");
            }
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await GetTokenAsync();
            System.Diagnostics.Debug.WriteLine($"[Auth] Token uzunluğu: {token?.Length ?? 0}");
            
            _httpClient.DefaultRequestHeaders.Authorization = !string.IsNullOrEmpty(token) 
                ? new AuthenticationHeaderValue("Bearer", token)
                : null;
        }

        private async Task<T?> SendAsync<T>(HttpMethod method, string endpoint, object? data = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[API] Başlangıç: {method} {_baseUrl}{endpoint}");
                await SetAuthHeaderAsync();

                var request = new HttpRequestMessage(method, $"{_baseUrl}{endpoint}");

                if (data != null)
                {
                    var json = JsonSerializer.Serialize(data);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    System.Diagnostics.Debug.WriteLine($"[API] İstek verisi: {json}");
                }

                System.Diagnostics.Debug.WriteLine($"[API] İstek gönderiliyor: {method} {_baseUrl}{endpoint}");

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"[API] Yanıt durumu: {response.StatusCode}");
                System.Diagnostics.Debug.WriteLine($"[API] Yanıt içeriği: {content}");

                if (response.IsSuccessStatusCode)
                {
                    try 
                    {
                        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        System.Diagnostics.Debug.WriteLine($"[API] Başarılı yanıt: {result}");
                        return result;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[API] Başarılı yanıt parse hatası: {ex.Message}");
                        return default;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[API] HTTP Hata: {response.StatusCode}");
                    try
                    {
                        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[API] Hata yanıtı parse hatası: {ex.Message}");
                        return default;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API] HTTP İstek hatası: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[API] İç hata: {ex.InnerException?.Message}");
                return default;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[API] Genel hata: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[API] Stack trace: {ex.StackTrace}");
                return default;
            }
        }

        // Auth methods
        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var response = await SendAsync<LoginResponse>(HttpMethod.Post, "/auth/login", request);
            if (response?.Token != null)
            {
                await SetTokenAsync(response.Token);
            }
            return response;
        }

        public async Task<MessageResponse?> RegisterAsync(RegisterRequest request)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Post, "/auth/register", request);
        }

        public async Task<MessageResponse?> ChangePasswordAsync(ChangePasswordRequest request)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Post, "/auth/change-password", request);
        }

        public async Task<MessageResponse?> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Post, "/auth/forgot-password", request);
        }

        public async Task<MessageResponse?> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Post, "/auth/reset-password", request);
        }

        public async Task<MessageResponse?> LogoutAsync()
        {
            var response = await SendAsync<MessageResponse>(HttpMethod.Post, "/auth/logout");
            ClearTokenAsync();
            return response;
        }

        // User methods
        public async Task<User?> GetCurrentUserProfileAsync()
        {
            return await SendAsync<User>(HttpMethod.Get, "/users/profile");
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await SendAsync<User>(HttpMethod.Get, $"/users/{username}");
        }

        public async Task<MessageResponse?> UpdateProfileAsync(UpdateProfileRequest request)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Put, "/users/profile", request);
        }

        public async Task<List<User>?> SearchUsersAsync(string query)
        {
            return await SendAsync<List<User>>(HttpMethod.Get, $"/users/search?q={query}");
        }

        public async Task<MessageResponse?> FollowUserAsync(string userId)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Post, $"/users/{userId}/follow");
        }

        public async Task<MessageResponse?> UnfollowUserAsync(string userId)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Delete, $"/users/{userId}/follow");
        }

        public async Task<User?> GetUserProfileAsync(string userId)
        {
            return await SendAsync<User>(HttpMethod.Get, $"/users/{userId}/profile");
        }

        public async Task<List<User>?> GetFollowersAsync(string userId)
        {
            return await SendAsync<List<User>>(HttpMethod.Get, $"/users/{userId}/followers");
        }

        public async Task<List<User>?> GetFollowingAsync(string userId)
        {
            return await SendAsync<List<User>>(HttpMethod.Get, $"/users/{userId}/following");
        }

        // Post methods
        public async Task<PostResponse?> CreatePostAsync(CreatePostRequest request)
        {
            return await SendAsync<PostResponse>(HttpMethod.Post, "/posts", request);
        }

        public async Task<MessageResponse?> UpdatePostAsync(int postId, UpdatePostRequest request)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Put, $"/posts/{postId}", request);
        }

        public async Task<MessageResponse?> DeletePostAsync(int postId)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Delete, $"/posts/{postId}");
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await SendAsync<Post>(HttpMethod.Get, $"/posts/{postId}");
        }

        public async Task<List<Post>?> GetAllPostsAsync(int page = 1, int pageSize = 10)
        {
            return await SendAsync<List<Post>>(HttpMethod.Get, $"/posts?page={page}&pageSize={pageSize}");
        }

        public async Task<List<Post>?> GetUserPostsAsync(string userId, int page = 1, int pageSize = 10)
        {
            return await SendAsync<List<Post>>(HttpMethod.Get, $"/users/{userId}/posts?page={page}&pageSize={pageSize}");
        }

        public async Task<List<Post>?> GetFeedAsync(int page = 1, int pageSize = 10)
        {
            System.Diagnostics.Debug.WriteLine($"ApiService: GetFeedAsync called - page={page}, pageSize={pageSize}");
            var result = await SendAsync<List<Post>>(HttpMethod.Get, $"/posts/feed?page={page}&pageSize={pageSize}");
            System.Diagnostics.Debug.WriteLine($"ApiService: GetFeedAsync result - {result?.Count ?? 0} posts returned");
            if (result != null)
            {
                foreach (var post in result)
                {
                    System.Diagnostics.Debug.WriteLine($"ApiService: Post - ID:{post.Id}, Content:'{post.Content}', User:{post.UserFullName}");
                }
            }
            return result;
        }

        public async Task<MessageResponse?> LikePostAsync(int postId)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Post, $"/posts/{postId}/like");
        }

        public async Task<MessageResponse?> UnlikePostAsync(int postId)
        {
            return await SendAsync<MessageResponse>(HttpMethod.Delete, $"/posts/{postId}/like");
        }
    }
} 