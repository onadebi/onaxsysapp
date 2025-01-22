using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using AppCore.Domain.AppCore.Models.Extensions;

namespace AppCore.Domain.Blog.Entities;

[Table(nameof(Posts))]
// [BsonIgnoreExtraElements]
public class Posts : CommonProperties
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; }

    [BsonRequired]
    [BsonElement(nameof(Title))]
    public required string Title { get; set; }

    [BsonRequired]
    [BsonElement(nameof(Content))]
    public required string Content { get; set; }

    [BsonElement(nameof(SlugUrl))]
    public string SlugUrl { get; set; }

    //[BsonRequired]
    [BsonElement(nameof(ContentExcerpt))]
    public required string ContentExcerpt { get; set; }

    [BsonRequired]
    [BsonElement(nameof(PostedBy))]
    public required Guid PostedBy { get; set; }

    [BsonElement(nameof(FeaturedImageUrl))]
    public Images? FeaturedImageUrl { get; set; }

    [BsonElement(nameof(CloudTags))]
    public List<string>? CloudTags { get; set; }

    [BsonElement(nameof(Likes))]
    public HashSet<Guid>? Likes { get; set; }

    [BsonElement(nameof(Comments))]
    public List<PostComments> Comments { get; set; } = new();

    [BsonRequired]
    [BsonElement(nameof(Categories))]
    public List<string> Categories { get; set; } = new();
}

public class Images
{
    public required string Url { get; set; }
    public required string Guid { get; set; }
}

