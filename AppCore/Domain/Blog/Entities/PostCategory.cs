using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;
using AppCore.Domain.AppCore.Models.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.Blog.Entities
{
    [Table(nameof(PostCategory), Schema = "blog")]
    public class PostCategory : CommonProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [BsonRequired]
        [BsonElement(nameof(Title))]
        public required string Title { get; set; }

        [BsonRequired]
        [BsonElement(nameof(TitleNormalized))]
        public string? TitleNormalized { get; set; }
        public required string Url { get; set; }
        public required string Slug { get; set; }

        [BsonElement(nameof(Description))]
        public string? Description { get; set; }

        public int Order { get; set; }
        public string? icon { get; set; }

        public PostCategory()
        {
            TitleNormalized = !string.IsNullOrWhiteSpace(Title) ? Title.ToUpper() : TitleNormalized;
        }
    }
}
