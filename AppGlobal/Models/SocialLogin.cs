namespace AppGlobal.Models;
public class SocialLogin
{
    public required bool IsSocialLogin { get; set; } = false;
    public string? SocialLoginAppName { get; set; }
    public string? token { get; set; } = null;
}

public static class SocialLoginPlatform
{
    public const string Google = "Google";
    public const string Clerk = "Clerk";
    public const string Twitter = "Twitter";
    public const string LinkedIn = "LinkedIn";
    public const string GitHub = "GitHub";
    public const string Microsoft = "Microsoft";
    public const string YouTube = "YouTube";
}