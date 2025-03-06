using AppCore.Domain.AppCore.Models.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Domain.AppCore.Models
{
    [Table(nameof(UserApp), Schema = "auth")]
    public class UserApp: CommonProperties
    {
        [Key]
        public string UserId { get; set; } = default!;

        public string? AppId { get; set; }

        public string? SocialPlatform { get; set; }

        public string? OAuthIdentity { get; set; }

        public string? OAuthGuid { get; set; }

        [Required]
        public List<string> UserRole { get; set; } = [];

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public UserProfile UserProfile { get; set; } = default!;

        public int CreditBalance { get; set; }
    }
}
