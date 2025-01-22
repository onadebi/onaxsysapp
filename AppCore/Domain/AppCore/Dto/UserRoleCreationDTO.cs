using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
namespace AppCore.Domain.AppCore.Dto
{
    public class UserRoleCreationDTO
    {
        [Required]
        [StringLength(100)]
        [BsonElement(nameof(RoleName))]
        public required string RoleName { get; set; }


        [Required]
        [StringLength(250)]
        [BsonElement(nameof(RoleDescription))]
        public string? RoleDescription { get; set; }
    }
}