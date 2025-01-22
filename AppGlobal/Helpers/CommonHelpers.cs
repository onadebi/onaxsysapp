using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AppGlobal.Helpers;

public static class CommonHelpers
{

    public static string GetIp(HttpContext httpContext)
    {
        string ip = httpContext.GetServerVariable("HTTP_X_FORWARDED_FOR") ?? string.Empty;
        if (string.IsNullOrEmpty(ip))
        {
            ip = httpContext.GetServerVariable("REMOTE_ADDR") ?? string.Empty;
        }
        return ip;
    }

    public static bool IsValidJson(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        try
        {
            using (JsonDocument.Parse(text))
            {
                return true;
            }
        }
        catch (JsonException)
        {
            return false;
        }
    }

}
