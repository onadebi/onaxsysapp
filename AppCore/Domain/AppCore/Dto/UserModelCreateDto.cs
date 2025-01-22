using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.AppCore.Dto
{

    public class UserModelCreateDto
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        [MinLength(6, ErrorMessage ="Password must have at least 6 characters.")]
        public required string Password { get; set; }

        [Required]
        public required string ConfirmPassword { get; set; }
    }


    public class UserAuthUpdatePasswordDto
    {
        [Required]
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
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Token { get; set; }

        [Required]
        public required string NewPassword { get; set; }

        [Required]
        public required string ConfirmNewPassword { get; set; }
    }

}
