using System.Collections.Concurrent;

namespace AppGlobal.Config;

public class AppSettings
{
    public string AppName { get; set; } = string.Empty;
    public string StartUpAssemblyName { get; set; } = string.Empty;
    public string AppKey { get; set; } = string.Empty;
    public bool LogToAppInsights { get; set; }
    public Encryption? Encryption { get; set; }
    public MessageBroker? MessageBroker { get; set; }
    public AzureBlobConfig AzureBlobConfig { get; set; } = default!;
    public required ExternalAPIs ExternalAPIs { get; set; }
    public AzKeyVault AzKeyVault { get; set; } = default!;
    public required SessionConfig SessionConfig { get; set; }
    public SpeechSynthesis SpeechSynthesis { get; set; } = default!;
    public required Prompts Prompts { get; set; }
}

public class ExternalAPIs
{
    public OpenAI? OpenAI { get; set; }
    public required GeminiApi GeminiApi { get; set; }
    public required YoutubeApi YoutubeApi { get; set; }
    public required GoogleOAuth GoogleOAuth { get; set; }
}

public class AzKeyVault
{
    public string KeyVaultUrl { get; set; } = string.Empty;
    public string ApplicationClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string VaultDirectoryTenantId { get; set; } = string.Empty;
}
public class Prompts
{
    public required string PromptCourseLayout { get; set; }
    public required string ChapterGenerationPrompt { get; set; }
}
public class OpenAI : UrlGeneric
{
    public string OpenAIKey { get; set; } = string.Empty;
}

public class GoogleOAuth : UrlGeneric
{
    public string ClientId { get; set; } = string.Empty;
}

public class GeminiApi : UrlGeneric
{
    public string GeminiApiApiKey { get; set; } = string.Empty;
}
public class YoutubeApi : UrlGeneric
{
    public string YoutubeApiKey { get; set; } = string.Empty;
}

public class SpeechSynthesis
{
    public required string SpeechKey { get; set; }
    public required string SpeechEndpoint { get; set; }
    public string SpeechLocation { get; set; }=  string.Empty;
}
public class AzureBlobConfig
{
    public string BlobStorageConstring { get; set; } = string.Empty;
    public string BlobReadAccessYear2099 { get; set; } = string.Empty;
    public string BlobStoragePath { get; set; } = string.Empty;
    public string DefaultContainerName { get; set; } = string.Empty;
}

public class MessageBroker
{
    public required RabbitMq RabbitMq { get; set; }
}
public class RabbitMq
{
    public required string ConString { get; set; }
}
public class Encryption
{
    public string Key { get; set; } = string.Empty;
    public HashSet<ConcurrentDictionary<string, string>> ThirdParty { get; set; } = [];
}
public class SessionConfig
{
    public required Auth Auth { get; set; }
}

public class Auth
{
    public int ExpireMinutes { get; set; }
    public bool HttpOnly { get; set; }
    public bool Secure { get; set; }
    public bool IsEssential { get; set; }
    public string token { get; set; } = string.Empty;
}

public class UrlGeneric
{
    public string Url { get; set; } = default!;
}