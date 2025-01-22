using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.Blog.Dto
{
    public class UpdatePostDto : CreatePostDto
    {
        [Required]
        [MaxLength(50)]
        public string Id { get; set; } = default!;
    }
}
