using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.AppCore.Dto
{
    public class EmailValidationDto
    {
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Token { get; set; }
        [Required]
        public required string UserGuid { get; set; }
    }
}
