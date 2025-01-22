using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AppCore.Domain.Blog.Dto
{
    public class PostCategoryDto: PostCategoryCreateDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string Id { get; set; }

       
    }

    public class PostCategoryCreateDto
    {
        [BsonRequired]
        [BsonElement(nameof(Title))]
        public required string Title { get; set; }

        [BsonElement(nameof(Description))]
        public string? Description { get; set; }
    }
}
