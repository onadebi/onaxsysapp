using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppCore.Domain.AppCore.Models.Extensions;

namespace AppCore.Domain.AppCore.Models
{
    [Table(nameof(UserRole),Schema ="auth")]
    [BsonIgnoreExtraElements]
    public class UserRole: CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [BsonElement(nameof(RoleName))]
        public required string RoleName { get; set; }


        [StringLength(250)]
        [BsonElement(nameof(RoleDescription))]
        public string? RoleDescription { get; set; }

        [StringLength(250)]
        [BsonElement(nameof(ConnectedApp))]
        public string? ConnectedApp { get; set; }

    }
}
