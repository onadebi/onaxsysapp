namespace AppGlobal.Models.SocialAuth;


public class GoogleOAuthResponse
{
    public string Audience { get; set; } = string.Empty;
    public List<string> AudienceAsList { get; set; } =[];
    public string Email { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public int ExpirationTimeSeconds { get; set; }
    public string FamilyName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string? HostedDomain { get; set; }
    public int IssuedAtTimeSeconds { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty;
    public string Locale { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Nonce { get; set; } = string.Empty;
    public int NotBeforeTimeSeconds { get; set; }
    public string? Picture { get; set; }
    public string? Prn { get; set; }
    public string? Scope { get; set; }
    public string? Subject { get; set; }
    public string? TargetAudience { get; set; }
    public string? Type { get; set; }
}