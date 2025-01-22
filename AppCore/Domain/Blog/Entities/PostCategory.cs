using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using AppCore.Domain.AppCore.Models.Extensions;

namespace AppCore.Domain.Blog.Entities
{
    [Table(nameof(PostCategory))]
    public class PostCategory : CommonProperties
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement(nameof(Title))]
        public string Title { get; set; }

        [BsonRequired]
        [BsonElement(nameof(TitleNormalized))]
        public string? TitleNormalized { get; set; }

        [BsonElement(nameof(Description))]
        public string? Description { get; set; }

        public PostCategory()
        {
            TitleNormalized = !string.IsNullOrWhiteSpace(Title) ? Title.ToUpper() : TitleNormalized;
        }
    }
}
