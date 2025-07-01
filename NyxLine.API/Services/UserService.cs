using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NyxLine.API.Data;
using NyxLine.API.DTOs;
using NyxLine.API.Models;

namespace NyxLine.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public UserService(UserManager<User> userManager, ApplicationDbContext context, IFileService fileService)
        {
            _userManager = userManager;
            _context = context;
            _fileService = fileService;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId, string? currentUserId = null)
        {
            var user = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            bool isFollowed = false;
            if (currentUserId != null)
            {
                isFollowed = await _context.Follows
                    .AnyAsync(f => f.FollowerId == currentUserId && f.FollowedId == userId);
            }

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfileImagePath = user.ProfileImagePath,
                IsGhost = user.IsGhost,
                IsAdmin = user.IsAdmin,
                PostsCount = user.Posts.Count,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                IsFollowedByCurrentUser = isFollowed,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserProfileDto?> GetUserByUserNameAsync(string userName, string? currentUserId = null)
        {
            var user = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null) return null;

            bool isFollowed = false;
            if (currentUserId != null)
            {
                isFollowed = await _context.Follows
                    .AnyAsync(f => f.FollowerId == currentUserId && f.FollowedId == user.Id);
            }

            return new UserProfileDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfileImagePath = user.ProfileImagePath,
                IsGhost = user.IsGhost,
                IsAdmin = user.IsAdmin,
                PostsCount = user.Posts.Count,
                FollowersCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                IsFollowedByCurrentUser = isFollowed,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<(bool Success, string Message)> UpdateProfileAsync(string userId, UpdateProfileDto updateDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return (false, "Kullanıcı bulunamadı");
                }

                user.FirstName = updateDto.FirstName;
                user.LastName = updateDto.LastName;
                user.Bio = updateDto.Bio;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = user.UserName;

                // Handle profile image upload
                if (updateDto.ProfileImage != null)
                {
                    if (!_fileService.IsValidImageFile(updateDto.ProfileImage))
                    {
                        return (false, "Geçersiz resim dosyası");
                    }

                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(user.ProfileImagePath))
                    {
                        _fileService.DeleteFile(user.ProfileImagePath);
                    }

                    var imagePath = await _fileService.SaveFileAsync(updateDto.ProfileImage, "profiles");
                    user.ProfileImagePath = imagePath;
                }
                // Handle Base64 profile image from MAUI
                else if (!string.IsNullOrEmpty(updateDto.ProfileImageBase64))
                {
                    try
                    {
                        Console.WriteLine($"[DEBUG] Base64 profil fotoğrafı işleniyor. Uzunluk: {updateDto.ProfileImageBase64.Length}");
                        
                        var imageBytes = Convert.FromBase64String(updateDto.ProfileImageBase64);
                        Console.WriteLine($"[DEBUG] Base64 decode edildi. Byte uzunluğu: {imageBytes.Length}");
                        
                        var fileName = $"profile_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
                        Console.WriteLine($"[DEBUG] Dosya adı: {fileName}");
                        
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(user.ProfileImagePath))
                        {
                            Console.WriteLine($"[DEBUG] Eski profil fotoğrafı siliniyor: {user.ProfileImagePath}");
                            _fileService.DeleteFile(user.ProfileImagePath);
                        }

                        var imagePath = await _fileService.SaveFileFromBytesAsync(imageBytes, fileName, "profiles");
                        Console.WriteLine($"[DEBUG] SaveFileFromBytesAsync sonucu: {imagePath}");
                        
                        if (imagePath == null)
                        {
                            Console.WriteLine("[ERROR] Profil fotoğrafı yüklenemedi - imagePath null");
                            return (false, "Profil fotoğrafı yüklenemedi");
                        }
                        
                        user.ProfileImagePath = imagePath;
                        Console.WriteLine($"[DEBUG] Kullanıcı ProfileImagePath güncellendi: {user.ProfileImagePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Profil fotoğrafı işlenirken hata: {ex.Message}");
                        Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                        return (false, $"Profil fotoğrafı işlenirken hata: {ex.Message}");
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return (true, "Profil başarıyla güncellendi");
                }

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Profil güncellenemedi: {errors}");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<List<UserProfileDto>> SearchUsersAsync(string searchTerm, string? currentUserId = null)
        {
            var query = _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .Where(u => u.UserName!.Contains(searchTerm) || 
                           u.FirstName.Contains(searchTerm) || 
                           u.LastName.Contains(searchTerm))
                .Take(20);

            var users = await query.ToListAsync();

            var userProfiles = new List<UserProfileDto>();
            foreach (var user in users)
            {
                bool isFollowed = false;
                if (currentUserId != null)
                {
                    isFollowed = await _context.Follows
                        .AnyAsync(f => f.FollowerId == currentUserId && f.FollowedId == user.Id);
                }

                userProfiles.Add(new UserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Bio = user.Bio,
                    ProfileImagePath = user.ProfileImagePath,
                    IsGhost = user.IsGhost,
                    IsAdmin = user.IsAdmin,
                    PostsCount = user.Posts.Count,
                    FollowersCount = user.Followers.Count,
                    FollowingCount = user.Following.Count,
                    IsFollowedByCurrentUser = isFollowed,
                    CreatedAt = user.CreatedAt
                });
            }

            return userProfiles;
        }

        public async Task<(bool Success, string Message)> FollowUserAsync(string followerId, string followedId)
        {
            try
            {
                if (followerId == followedId)
                {
                    return (false, "Kendinizi takip edemezsiniz");
                }

                var follower = await _userManager.FindByIdAsync(followerId);
                var followed = await _userManager.FindByIdAsync(followedId);

                if (follower == null || followed == null)
                {
                    return (false, "Kullanıcı bulunamadı");
                }

                if (follower.IsGhost)
                {
                    return (false, "Hayalet kullanıcılar başka kullanıcıları takip edemez");
                }

                var existingFollow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);

                if (existingFollow != null)
                {
                    return (false, "Bu kullanıcıyı zaten takip ediyorsunuz");
                }

                var follow = new Follow
                {
                    FollowerId = followerId,
                    FollowedId = followedId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = follower.UserName
                };

                _context.Follows.Add(follow);
                await _context.SaveChangesAsync();

                return (true, "Kullanıcı başarıyla takip edildi");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UnfollowUserAsync(string followerId, string followedId)
        {
            try
            {
                var follow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);

                if (follow == null)
                {
                    return (false, "Bu kullanıcıyı takip etmiyorsunuz");
                }

                _context.Follows.Remove(follow);
                await _context.SaveChangesAsync();

                return (true, "Kullanıcı takipten çıkarıldı");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<List<UserProfileDto>> GetFollowersAsync(string userId, string? currentUserId = null)
        {
            var followers = await _context.Follows
                .Where(f => f.FollowedId == userId)
                .Include(f => f.Follower)
                    .ThenInclude(u => u.Posts)
                .Include(f => f.Follower)
                    .ThenInclude(u => u.Followers)
                .Include(f => f.Follower)
                    .ThenInclude(u => u.Following)
                .Select(f => f.Follower)
                .ToListAsync();

            var userProfiles = new List<UserProfileDto>();
            foreach (var user in followers)
            {
                bool isFollowed = false;
                if (currentUserId != null)
                {
                    isFollowed = await _context.Follows
                        .AnyAsync(f => f.FollowerId == currentUserId && f.FollowedId == user.Id);
                }

                userProfiles.Add(new UserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Bio = user.Bio,
                    ProfileImagePath = user.ProfileImagePath,
                    IsGhost = user.IsGhost,
                    IsAdmin = user.IsAdmin,
                    PostsCount = user.Posts.Count,
                    FollowersCount = user.Followers.Count,
                    FollowingCount = user.Following.Count,
                    IsFollowedByCurrentUser = isFollowed,
                    CreatedAt = user.CreatedAt
                });
            }

            return userProfiles;
        }

        public async Task<List<UserProfileDto>> GetFollowingAsync(string userId, string? currentUserId = null)
        {
            var following = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Followed)
                    .ThenInclude(u => u.Posts)
                .Include(f => f.Followed)
                    .ThenInclude(u => u.Followers)
                .Include(f => f.Followed)
                    .ThenInclude(u => u.Following)
                .Select(f => f.Followed)
                .ToListAsync();

            var userProfiles = new List<UserProfileDto>();
            foreach (var user in following)
            {
                bool isFollowed = false;
                if (currentUserId != null)
                {
                    isFollowed = await _context.Follows
                        .AnyAsync(f => f.FollowerId == currentUserId && f.FollowedId == user.Id);
                }

                userProfiles.Add(new UserProfileDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Bio = user.Bio,
                    ProfileImagePath = user.ProfileImagePath,
                    IsGhost = user.IsGhost,
                    IsAdmin = user.IsAdmin,
                    PostsCount = user.Posts.Count,
                    FollowersCount = user.Followers.Count,
                    FollowingCount = user.Following.Count,
                    IsFollowedByCurrentUser = isFollowed,
                    CreatedAt = user.CreatedAt
                });
            }

            return userProfiles;
        }
    }
} 