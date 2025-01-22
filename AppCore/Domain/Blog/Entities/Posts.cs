using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppCore.Domain.AppCore.Models.Extensions;

namespace AppCore.Domain.Blog.Entities;

[Table(nameof(Posts), Schema = "Blog")]
// [BsonIgnoreExtraElements]
public class Posts : CommonProperties
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid PostId { get; set; } = Guid.NewGuid();

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Content { get; set; }

    public required string SlugUrl { get; set; }

    //[Required]
    public required string ContentExcerpt { get; set; }

    [Required]
    public required Guid PostedBy { get; set; }

    public Images? FeaturedImageUrl { get; set; }

    // public List<string>? CloudTags { get; set; }

    // public HashSet<Guid>? Likes { get; set; }

    // public List<PostComments> Comments { get; set; } = new();

    [Required]
    public List<string> Categories { get; set; } = new();
}

public class Images
{
    public required string Url { get; set; }
    public required string Guid { get; set; }
}

