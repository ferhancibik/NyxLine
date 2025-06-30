using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NyxLine.API.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string FollowerId { get; set; } = string.Empty;
        
        [Required]
        public string FollowedId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }

        // Navigation Properties
        [ForeignKey("FollowerId")]
        public virtual User Follower { get; set; } = null!;
        
        [ForeignKey("FollowedId")]
        public virtual User Followed { get; set; } = null!;
    }
} 