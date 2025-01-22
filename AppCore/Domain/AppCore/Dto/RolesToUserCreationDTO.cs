using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.AppCore.Dto
{
    public class RolesToUserCreationDTO
    {
        [Required]
        public int UserProfileId { get; set; }
        public List<int> UserRoleIds { get; set; } = [];
    }
}
