using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.AppCore.Dto
{
    public class MessageTemplateCreationDTO
    {
        [Required]
        [StringLength(100)]
        [BsonElement(nameof(TemplateName))]
        public required string TemplateName { get; set; }

        [Required]
        [StringLength(100)]
        [BsonElement(nameof(Description))]
        public required string Description { get; set; }
    }
}
