using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AppCore.Domain.Blog.Dto
{
    public class PostCategoryDto: PostCategoryCreateDto
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

       
    }

    public class PostCategoryCreateDto
    {
        [BsonRequired]
        [BsonElement(nameof(Title))]
        public string Title { get; set; }

        [BsonElement(nameof(Description))]
        public string Description { get; set; }
    }
}
