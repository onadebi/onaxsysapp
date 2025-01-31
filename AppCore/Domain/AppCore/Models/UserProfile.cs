using OnaxTools.Dto.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppCore.Domain.AppCore.Models
{
    [Table(nameof(UserProfile), Schema = "auth")]
    public class UserProfile: AppUserIdentity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override long Id { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public required string FirstName { get; set; }

        [StringLength(maximumLength: 100)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(maximumLength: 100)]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(maximumLength: 100)]
        public override required string Email { get; set; }
        [Required]
        [StringLength(maximumLength: 100)]
        public string? Password { get; set; }

        [Required]
        public override DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsEmailConfirmed { get; set; } = false;
        public bool IsDeactivated { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        [StringLength(maximumLength: 100)]
        public override string Guid { get; set; } = System.Guid.NewGuid().ToString();


        public bool IsSocialLogin { get; set; } = false;

        [Column(TypeName = "varchar(250)")]
        public string? SocialLoginPlatform { get; set; }

        public string? UserProfileImage { get; set; } = null;

        [Required]
        public string? Username { get; set; }

        public DateTime LastLoginDate { get; set; } = DateTime.UtcNow;

        public UserProfile()
        {
           Username = string.IsNullOrWhiteSpace(Username) ? Email : Username;
        }

        #region Navigational properties
        public virtual ICollection<UserApp> UserProfileUserApps { get; set; } = [];

        #endregion

    }
}