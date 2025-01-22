using AppCore.Domain.AppCore.Config;
using MongoDB.Bson.Serialization.Attributes;

namespace AppCore.Domain.AppCore.Models.Extensions
{
    public class CommonProperties
    {
        [BsonElement(nameof(IsActive))]
        public virtual bool IsActive { get; set; } = true;
        [BsonElement(nameof(IsDeleted))]
        public virtual bool IsDeleted { get; set; } = false;
        [BsonElement(nameof(CreatedAt))]
        public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement(nameof(UpdatedAt))]
        public virtual DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [BsonElement(nameof(CreatedBy))]
        public virtual string CreatedBy { get; set; } = AppConstants.AppSystem;

        [BsonElement(nameof(ModifiedBy))]
        public virtual string ModifiedBy { get; set; } = AppConstants.AppSystem;
    }
}
