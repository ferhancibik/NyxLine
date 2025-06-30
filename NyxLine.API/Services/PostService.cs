using Microsoft.EntityFrameworkCore;
using NyxLine.API.Data;
using NyxLine.API.DTOs;
using NyxLine.API.Models;

namespace NyxLine.API.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public PostService(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<(bool Success, string Message, PostResponseDto? Post)> CreatePostAsync(string userId, CreatePostDto createPostDto)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return (false, "Kullanıcı bulunamadı", null);
                }

                if (user.IsGhost)
                {
                    return (false, "Hayalet kullanıcılar post oluşturamaz", null);
                }

                string? imagePath = null;
                
                // IFormFile ile gönderilen resim
                if (createPostDto.Image != null)
                {
                    if (!_fileService.IsValidImageFile(createPostDto.Image))
                    {
                        return (false, "Geçersiz resim dosyası", null);
                    }

                    imagePath = await _fileService.SaveFileAsync(createPostDto.Image, "posts");
                    if (imagePath == null)
                    {
                        return (false, "Resim yüklenemedi", null);
                    }
                }
                // Base64 string ile gönderilen resim (MAUI'den)
                else if (!string.IsNullOrEmpty(createPostDto.ImageBase64))
                {
                    try
                    {
                        var imageBytes = Convert.FromBase64String(createPostDto.ImageBase64);
                        var fileName = createPostDto.FileName ?? $"post_image_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg";
                        
                        imagePath = await _fileService.SaveFileFromBytesAsync(imageBytes, fileName, "posts");
                        if (imagePath == null)
                        {
                            return (false, "Resim yüklenemedi", null);
                        }
                    }
                    catch (Exception ex)
                    {
                        return (false, $"Base64 resim işlenirken hata: {ex.Message}", null);
                    }
                }

                var post = new Post
                {
                    Content = createPostDto.Content,
                    ImagePath = imagePath,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = user.UserName
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                var postResponse = await GetPostByIdAsync(post.Id, userId);
                return (true, "Post başarıyla oluşturuldu", postResponse);
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdatePostAsync(int postId, string userId, UpdatePostDto updatePostDto)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
                if (post == null)
                {
                    return (false, "Post bulunamadı veya yetkiniz yok");
                }

                post.Content = updatePostDto.Content;
                post.UpdatedAt = DateTime.UtcNow;
                post.UpdatedBy = userId;

                await _context.SaveChangesAsync();
                return (true, "Post başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeletePostAsync(int postId, string userId)
        {
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);
                if (post == null)
                {
                    return (false, "Post bulunamadı veya yetkiniz yok");
                }

                // Delete associated image if exists
                if (!string.IsNullOrEmpty(post.ImagePath))
                {
                    _fileService.DeleteFile(post.ImagePath);
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                return (true, "Post başarıyla silindi");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<PostResponseDto?> GetPostByIdAsync(int postId, string? currentUserId = null)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            bool isLiked = false;
            if (currentUserId != null)
            {
                isLiked = await _context.Likes
                    .AnyAsync(l => l.PostId == postId && l.UserId == currentUserId);
            }

            return new PostResponseDto
            {
                Id = post.Id,
                Content = post.Content,
                ImagePath = post.ImagePath,
                UserId = post.UserId,
                UserName = post.User.UserName ?? "",
                UserFullName = $"{post.User.FirstName} {post.User.LastName}",
                UserProfileImage = post.User.ProfileImagePath,
                LikeCount = post.Likes.Count,
                IsLikedByCurrentUser = isLiked,
                CreatedAt = post.CreatedAt
            };
        }

        public async Task<List<PostResponseDto>> GetUserPostsAsync(string userId, string? currentUserId = null, int page = 1, int pageSize = 10)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = new List<PostResponseDto>();
            foreach (var post in posts)
            {
                bool isLiked = false;
                if (currentUserId != null)
                {
                    isLiked = await _context.Likes
                        .AnyAsync(l => l.PostId == post.Id && l.UserId == currentUserId);
                }

                postDtos.Add(new PostResponseDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImagePath = post.ImagePath,
                    UserId = post.UserId,
                    UserName = post.User.UserName ?? "",
                    UserFullName = $"{post.User.FirstName} {post.User.LastName}",
                    UserProfileImage = post.User.ProfileImagePath,
                    LikeCount = post.Likes.Count,
                    IsLikedByCurrentUser = isLiked,
                    CreatedAt = post.CreatedAt
                });
            }

            return postDtos;
        }

        public async Task<List<PostResponseDto>> GetFeedAsync(string userId, int page = 1, int pageSize = 10)
        {
            // Get posts from followed users
            var followedUserIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId)
                .ToListAsync();

            // Include user's own posts
            followedUserIds.Add(userId);

            IQueryable<Post> postsQuery;
            
            // If user doesn't follow anyone (except themselves), show all posts
            if (followedUserIds.Count == 1) // Only contains the user's own ID
            {
                postsQuery = _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Likes);
            }
            else
            {
                postsQuery = _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Likes)
                    .Where(p => followedUserIds.Contains(p.UserId));
            }

            var posts = await postsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = new List<PostResponseDto>();
            foreach (var post in posts)
            {
                bool isLiked = await _context.Likes
                    .AnyAsync(l => l.PostId == post.Id && l.UserId == userId);

                postDtos.Add(new PostResponseDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImagePath = post.ImagePath,
                    UserId = post.UserId,
                    UserName = post.User.UserName ?? "",
                    UserFullName = $"{post.User.FirstName} {post.User.LastName}",
                    UserProfileImage = post.User.ProfileImagePath,
                    LikeCount = post.Likes.Count,
                    IsLikedByCurrentUser = isLiked,
                    CreatedAt = post.CreatedAt
                });
            }

            return postDtos;
        }

        public async Task<(bool Success, string Message)> LikePostAsync(int postId, string userId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(postId);
                if (post == null)
                {
                    return (false, "Post bulunamadı");
                }

                var existingLike = await _context.Likes
                    .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

                if (existingLike != null)
                {
                    return (false, "Bu postu zaten beğendiniz");
                }

                var like = new Like
                {
                    PostId = postId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };

                _context.Likes.Add(like);
                await _context.SaveChangesAsync();

                return (true, "Post beğenildi");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> UnlikePostAsync(int postId, string userId)
        {
            try
            {
                var like = await _context.Likes
                    .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

                if (like == null)
                {
                    return (false, "Bu postu beğenmemişsiniz");
                }

                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();

                return (true, "Beğeni kaldırıldı");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }

        public async Task<List<PostResponseDto>> GetAllPostsAsync(string? currentUserId = null, int page = 1, int pageSize = 10)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = new List<PostResponseDto>();
            foreach (var post in posts)
            {
                bool isLiked = false;
                if (currentUserId != null)
                {
                    isLiked = await _context.Likes
                        .AnyAsync(l => l.PostId == post.Id && l.UserId == currentUserId);
                }

                postDtos.Add(new PostResponseDto
                {
                    Id = post.Id,
                    Content = post.Content,
                    ImagePath = post.ImagePath,
                    UserId = post.UserId,
                    UserName = post.User.UserName ?? "",
                    UserFullName = $"{post.User.FirstName} {post.User.LastName}",
                    UserProfileImage = post.User.ProfileImagePath,
                    LikeCount = post.Likes.Count,
                    IsLikedByCurrentUser = isLiked,
                    CreatedAt = post.CreatedAt
                });
            }

            return postDtos;
        }
    }
} 