namespace AppCore.Domain.Blog.Entities;

public class PostComments
{
    public required string Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public required PostedBy PostedBy { get; set; }
}

public class PostedBy
{
    public required Guid UserGuid { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
}
