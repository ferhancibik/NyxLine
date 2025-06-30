using NyxLine.API.DTOs;
using NyxLine.API.Models;

namespace NyxLine.API.Services
{
    public interface IPostService
    {
        Task<(bool Success, string Message, PostResponseDto? Post)> CreatePostAsync(string userId, CreatePostDto createPostDto);
        Task<(bool Success, string Message)> UpdatePostAsync(int postId, string userId, UpdatePostDto updatePostDto);
        Task<(bool Success, string Message)> DeletePostAsync(int postId, string userId);
        Task<PostResponseDto?> GetPostByIdAsync(int postId, string? currentUserId = null);
        Task<List<PostResponseDto>> GetUserPostsAsync(string userId, string? currentUserId = null, int page = 1, int pageSize = 10);
        Task<List<PostResponseDto>> GetFeedAsync(string userId, int page = 1, int pageSize = 10);
        Task<(bool Success, string Message)> LikePostAsync(int postId, string userId);
        Task<(bool Success, string Message)> UnlikePostAsync(int postId, string userId);
        Task<List<PostResponseDto>> GetAllPostsAsync(string? currentUserId = null, int page = 1, int pageSize = 10);
    }
} 