using System.ComponentModel.DataAnnotations;

namespace AppCore.Domain.Blog.Dto
{
    public class DeletePostDto
    {
        [Required]
        [MaxLength(50)]
        public string Id { get; set; }
    }
}
