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
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op.User)
                .Include(p => p.Reposts)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null) return null;

            bool isLiked = false;
            bool isReposted = false;
            if (currentUserId != null)
            {
                isLiked = await _context.Likes
                    .AnyAsync(l => l.PostId == postId && l.UserId == currentUserId);

                isReposted = await _context.Posts
                    .AnyAsync(p => p.OriginalPostId == postId && p.UserId == currentUserId);
            }

            var response = new PostResponseDto
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
                CreatedAt = post.CreatedAt,
                Type = post.Type,
                NewsTitle = post.NewsTitle,
                IsUserAdmin = post.User.IsAdmin,
                IsRepost = post.IsRepost,
                OriginalPostId = post.OriginalPostId,
                RepostCount = post.Reposts.Count,
                IsRepostedByCurrentUser = isReposted
            };

            if (post.OriginalPost != null)
            {
                response.OriginalPost = new PostResponseDto
                {
                    Id = post.OriginalPost.Id,
                    Content = post.OriginalPost.Content,
                    ImagePath = post.OriginalPost.ImagePath,
                    UserId = post.OriginalPost.UserId,
                    UserName = post.OriginalPost.User.UserName ?? "",
                    UserFullName = $"{post.OriginalPost.User.FirstName} {post.OriginalPost.User.LastName}",
                    UserProfileImage = post.OriginalPost.User.ProfileImagePath,
                    CreatedAt = post.OriginalPost.CreatedAt,
                    Type = post.OriginalPost.Type
                };
            }

            return response;
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
                    CreatedAt = post.CreatedAt,
                    Type = post.Type,
                    NewsTitle = post.NewsTitle,
                    IsUserAdmin = post.User.IsAdmin
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
                    .Include(p => p.Likes)
                    .Where(p => p.Type == PostType.Regular); // Sadece normal gönderiler
            }
            else
            {
                postsQuery = _context.Posts
                    .Include(p => p.User)
                    .Include(p => p.Likes)
                    .Where(p => followedUserIds.Contains(p.UserId) && p.Type == PostType.Regular); // Sadece normal gönderiler
            }

            var posts = await postsQuery
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var postDtos = new List<PostResponseDto>();
            foreach (var post in posts)
            {
                // Haber tipindeki gönderileri atla
                if (post.Type == PostType.News) continue;

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
                    CreatedAt = post.CreatedAt,
                    Type = post.Type,
                    NewsTitle = post.NewsTitle,
                    IsUserAdmin = post.User.IsAdmin
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
                .Where(p => p.Type == PostType.Regular) // Sadece normal gönderiler
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
                    CreatedAt = post.CreatedAt,
                    Type = post.Type
                });
            }

            return postDtos;
        }

        // Repost metodları
        public async Task<(bool Success, string Message, PostResponseDto? Post)> RepostAsync(int postId, string userId, string? content = null)
        {
            try
            {
                var originalPost = await _context.Posts
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (originalPost == null)
                {
                    return (false, "Gönderi bulunamadı", null);
                }

                // Kullanıcının kendi gönderisini repost etmesini engelle
                if (originalPost.UserId == userId)
                {
                    return (false, "Kendi gönderinizi tekrar paylaşamazsınız", null);
                }

                // Kullanıcının aynı gönderiyi tekrar repost etmesini engelle
                var existingRepost = await _context.Posts
                    .FirstOrDefaultAsync(p => p.OriginalPostId == postId && p.UserId == userId);

                if (existingRepost != null)
                {
                    return (false, "Bu gönderiyi zaten paylaştınız", null);
                }

                var repost = new Post
                {
                    Content = content ?? string.Empty,
                    UserId = userId,
                    OriginalPostId = postId,
                    Type = PostType.Regular,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Posts.Add(repost);
                await _context.SaveChangesAsync();

                var postResponse = await GetPostByIdAsync(repost.Id, userId);
                return (true, "Gönderi başarıyla paylaşıldı", postResponse);
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UndoRepostAsync(int postId, string userId)
        {
            try
            {
                var repost = await _context.Posts
                    .FirstOrDefaultAsync(p => p.OriginalPostId == postId && p.UserId == userId);

                if (repost == null)
                {
                    return (false, "Bu gönderiyi paylaşmamışsınız");
                }

                _context.Posts.Remove(repost);
                await _context.SaveChangesAsync();

                return (true, "Paylaşım kaldırıldı");
            }
            catch (Exception ex)
            {
                return (false, $"Bir hata oluştu: {ex.Message}");
            }
        }
    }
} 