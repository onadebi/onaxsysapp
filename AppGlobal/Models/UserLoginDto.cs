namespace AppGlobal.Models;
public class UserLoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public SocialLogin? SocialLogin { get; set; }
}
