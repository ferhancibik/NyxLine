using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NyxLine.API.Data;
using NyxLine.API.DTOs;
using NyxLine.API.Models;

namespace NyxLine.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;

        public AdminService(ApplicationDbContext context, UserManager<User> userManager, IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<bool> IsAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.IsAdmin == true && user?.IsBanned == false;
        }

        public async Task<List<PostResponseDto>> GetAllPostsAsync(int page, int pageSize)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImagePath = p.ImagePath,
                    UserId = p.UserId,
                    UserName = p.User.UserName ?? "",
                    UserFullName = $"{p.User.FirstName} {p.User.LastName}",
                    UserProfileImage = p.User.ProfileImagePath,
                    LikeCount = p.Likes.Count,
                    CreatedAt = p.CreatedAt,
                    Type = p.Type,
                    NewsTitle = p.NewsTitle,
                    IsUserAdmin = p.User.IsAdmin
                })
                .ToListAsync();

            return posts;
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                Console.WriteLine($"Post not found with ID: {postId}");
                return false;
            }

            // Gönderi resmini sil
            if (!string.IsNullOrEmpty(post.ImagePath))
            {
                await _fileService.DeleteFileAsync(post.ImagePath);
            }

            try
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Post successfully deleted with ID: {postId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post with ID {postId}: {ex.Message}");
                return false;
            }
        }

        public async Task<List<User>> GetAllUsersAsync(int page, int pageSize)
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> BanUserAsync(string adminId, AdminBanUserDto dto)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin?.IsAdmin != true) return false;

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null || user.IsAdmin) return false; // Admin'ler banlanamaz

            user.IsBanned = true;
            user.BannedAt = DateTime.UtcNow;
            user.BannedBy = adminId;
            user.BanReason = dto.Reason;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UnbanUserAsync(string adminId, AdminUnbanUserDto dto)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin?.IsAdmin != true) return false;

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null) return false;

            user.IsBanned = false;
            user.BannedAt = null;
            user.BannedBy = null;
            user.BanReason = null;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string adminId, string userId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin?.IsAdmin != true) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsAdmin) return false; // Admin'ler silinemez

            // Kullanıcının gönderilerini ve dosyalarını sil
            var userPosts = await _context.Posts.Where(p => p.UserId == userId).ToListAsync();
            foreach (var post in userPosts)
            {
                if (!string.IsNullOrEmpty(post.ImagePath))
                {
                    await _fileService.DeleteFileAsync(post.ImagePath);
                }
            }

            // Profil resmini sil
            if (!string.IsNullOrEmpty(user.ProfileImagePath))
            {
                await _fileService.DeleteFileAsync(user.ProfileImagePath);
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> MakeAdminAsync(string adminId, string userId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin?.IsAdmin != true) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.IsAdmin = true;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> RemoveAdminAsync(string adminId, string userId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin?.IsAdmin != true) return false;

            // Kendini admin'likten çıkaramaz
            if (adminId == userId) return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            user.IsAdmin = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<PostResponseDto?> CreateNewsAsync(string adminId, CreatePostDto dto)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (admin?.IsAdmin != true) return null;

            string? imagePath = null;
            if (!string.IsNullOrEmpty(dto.ImageBase64))
            {
                imagePath = await _fileService.SaveBase64ImageAsync(dto.ImageBase64, "posts", dto.FileName);
            }

            var newsPost = new Post
            {
                Content = dto.Content,
                ImagePath = imagePath,
                UserId = adminId,
                Type = PostType.News,
                NewsTitle = dto.NewsTitle,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Posts.Add(newsPost);
            await _context.SaveChangesAsync();

            return new PostResponseDto
            {
                Id = newsPost.Id,
                Content = newsPost.Content,
                ImagePath = newsPost.ImagePath,
                UserId = newsPost.UserId,
                UserName = admin.UserName ?? "",
                UserFullName = $"{admin.FirstName} {admin.LastName}",
                UserProfileImage = admin.ProfileImagePath,
                LikeCount = 0,
                IsLikedByCurrentUser = false,
                CreatedAt = newsPost.CreatedAt,
                Type = newsPost.Type,
                NewsTitle = newsPost.NewsTitle,
                IsUserAdmin = true
            };
        }

        public async Task<List<PostResponseDto>> GetNewsAsync(int page, int pageSize)
        {
            var newsPosts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Where(p => p.Type == PostType.News)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PostResponseDto
                {
                    Id = p.Id,
                    Content = p.Content,
                    ImagePath = p.ImagePath,
                    UserId = p.UserId,
                    UserName = p.User.UserName ?? "",
                    UserFullName = $"{p.User.FirstName} {p.User.LastName}",
                    UserProfileImage = p.User.ProfileImagePath,
                    LikeCount = p.Likes.Count,
                    CreatedAt = p.CreatedAt,
                    Type = p.Type,
                    NewsTitle = p.NewsTitle,
                    IsUserAdmin = p.User.IsAdmin
                })
                .ToListAsync();

            return newsPosts;
        }
    }
} 