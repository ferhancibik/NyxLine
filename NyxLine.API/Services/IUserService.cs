using NyxLine.API.DTOs;
using NyxLine.API.Models;

namespace NyxLine.API.Services
{
    public interface IUserService
    {
        Task<UserProfileDto?> GetUserProfileAsync(string userId, string? currentUserId = null);
        Task<UserProfileDto?> GetUserByUserNameAsync(string userName, string? currentUserId = null);
        Task<(bool Success, string Message)> UpdateProfileAsync(string userId, UpdateProfileDto updateDto);
        Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm, string? currentUserId = null);
        Task<(bool Success, string Message)> FollowUserAsync(string followerId, string followedId);
        Task<(bool Success, string Message)> UnfollowUserAsync(string followerId, string followedId);
        Task<List<UserProfileDto>> GetFollowersAsync(string userId, string? currentUserId = null);
        Task<List<UserProfileDto>> GetFollowingAsync(string userId, string? currentUserId = null);
    }
} 