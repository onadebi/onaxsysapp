namespace AppGlobal.Config;

public static class AppConstants
{
    public static string CookieUserId { get; set; } = "token";
    public static string CookieSocialToken { get; set; } = "_onx_appuser_session";
    public static string AppSystem { get; set; } = nameof(AppSystem);
    public static string EmailTemplateConfirmEmail { get; set; } = "ConfirmEmail";
    public static string EmailTemplateForgottenPasswordRequest { get; set; } = "ForgottenPasswordRequest";
}
