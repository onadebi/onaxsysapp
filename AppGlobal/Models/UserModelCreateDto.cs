using System.ComponentModel.DataAnnotations;

namespace AppGlobal.Models
{
    public class UserModelCreateDto
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        [EmailAddress]
        public required string Email { get; set; }
        
        //[MinLength(6, ErrorMessage = "Password must have at least 6 characters.")]
        public required string Password { get; set; }

        public required string ConfirmPassword { get; set; }

        public string? UserProfileImage { get; set; }

        public required SocialLogin SocialLogin { get; set; }

    }


    public class UserAuthUpdatePasswordDto
    {
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        public required string NewPassword { get; set; }
        [Required]
        public required string ConfirmNewPassword { get; set; }
    }


    public class UserAuthChangeForgottenPasswordDto
    {
        [EmailAddress]
        public required string Email { get; set; }

        public required string Token { get; set; }

        public required string NewPassword { get; set; }

        public required string ConfirmNewPassword { get; set; }
    }

}
