namespace AppGlobal.Models;
public class UserLoginDto
{
    public required string Email { get; set; }
    public string? Password { get; set; }
    public required SocialLogin SocialLogin { get; set; }
}
