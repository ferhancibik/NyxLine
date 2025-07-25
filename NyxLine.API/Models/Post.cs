using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NyxLine.API.Models
{
    public enum PostType
    {
        Regular = 0,
        News = 1
    }

    public class Post
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = string.Empty;
        
        public string? ImagePath { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public PostType Type { get; set; } = PostType.Regular;
        
        [MaxLength(200)]
        public string? NewsTitle { get; set; }

        // Repost özellikleri
        public int? OriginalPostId { get; set; }
        public virtual Post? OriginalPost { get; set; }
        public virtual ICollection<Post> Reposts { get; set; } = new List<Post>();
        public bool IsRepost => OriginalPostId.HasValue;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
} 