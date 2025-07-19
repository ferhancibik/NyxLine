using NyxLine.MAUI.Models;

namespace NyxLine.MAUI.Services
{
    public interface IApiService
    {
        // Auth methods
        Task<LoginResponse?> LoginAsync(LoginRequest request);
        Task<MessageResponse?> RegisterAsync(RegisterRequest request);
        Task<MessageResponse?> ChangePasswordAsync(ChangePasswordRequest request);
        Task<MessageResponse?> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<MessageResponse?> ResetPasswordAsync(ResetPasswordRequest request);
        Task<MessageResponse?> LogoutAsync();

        // User methods
        Task<User?> GetCurrentUserProfileAsync();
        Task<User?> GetUserByUsernameAsync(string username);
        Task<MessageResponse?> UpdateProfileAsync(UpdateProfileRequest request);
        Task<List<User>?> SearchUsersAsync(string query);
        Task<MessageResponse?> FollowUserAsync(string userId);
        Task<MessageResponse?> UnfollowUserAsync(string userId);
        Task<User?> GetUserProfileAsync(string userId);
        Task<List<User>?> GetFollowersAsync(string userId);
        Task<List<User>?> GetFollowingAsync(string userId);

        // Post methods
        Task<PostResponse?> CreatePostAsync(CreatePostRequest request);
        Task<MessageResponse?> UpdatePostAsync(int postId, UpdatePostRequest request);
        Task<MessageResponse?> DeletePostAsync(int postId);
        Task<Post?> GetPostByIdAsync(int postId);
        Task<List<Post>?> GetAllPostsAsync(int page = 1, int pageSize = 10);
        Task<List<Post>?> GetUserPostsAsync(string userId, int page = 1, int pageSize = 10);
        Task<List<Post>?> GetFeedAsync(int page = 1, int pageSize = 10);
        Task<MessageResponse?> LikePostAsync(int postId);
        Task<MessageResponse?> UnlikePostAsync(int postId);
        Task<List<Post>?> GetNewsAsync(int page = 1, int pageSize = 20);

        // Repost methods
        Task<MessageResponse?> RepostAsync(int postId, string? content = null);
        Task<MessageResponse?> UndoRepostAsync(int postId);

        // Admin methods
        Task<PostResponse?> CreateNewsAsync(CreatePostRequest request);
        Task<MessageResponse?> AdminDeletePostAsync(int postId);
        Task<List<Post>?> GetAllPostsForAdminAsync(int page = 1, int pageSize = 20);
    }
} 