using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NyxLine.API.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Bio { get; set; }
        
        public string? ProfileImagePath { get; set; }
        
        public bool IsGhost { get; set; } = false;
        
        public bool IsAdmin { get; set; } = false;
        
        public bool IsBanned { get; set; } = false;
        
        public DateTime? BannedAt { get; set; }
        
        public string? BannedBy { get; set; }
        
        public string? BanReason { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
        public virtual ICollection<Follow> Following { get; set; } = new List<Follow>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
} 