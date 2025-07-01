using NyxLine.API.DTOs;
using NyxLine.API.Models;

namespace NyxLine.API.Services
{
    public interface IAdminService
    {
        Task<bool> IsAdminAsync(string userId);
        Task<List<PostResponseDto>> GetAllPostsAsync(int page, int pageSize);
        Task<bool> DeletePostAsync(int postId);
        Task<List<User>> GetAllUsersAsync(int page, int pageSize);
        Task<bool> BanUserAsync(string adminId, AdminBanUserDto dto);
        Task<bool> UnbanUserAsync(string adminId, AdminUnbanUserDto dto);
        Task<bool> DeleteUserAsync(string adminId, string userId);
        Task<bool> MakeAdminAsync(string adminId, string userId);
        Task<bool> RemoveAdminAsync(string adminId, string userId);
        Task<PostResponseDto?> CreateNewsAsync(string adminId, CreatePostDto dto);
        Task<List<PostResponseDto>> GetNewsAsync(int page, int pageSize);
    }
} 