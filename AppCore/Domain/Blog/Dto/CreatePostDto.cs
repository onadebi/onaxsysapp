using AppCore.Domain.Blog.Entities;
using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.Blog.Dto
{

    public class CreatePostDto
    {
        [Required]
        [MaxLength(500)]
        public required string Title { get; set; }
        [Required]
        [MaxLength(2500)]
        public required string Content { get; set; }
        public Images FeaturedImageUrl { get; set; }
        public List<string> CloudTags { get; set; } =[];
        public List<string> Categories { get; set; } = new();
    }
}
