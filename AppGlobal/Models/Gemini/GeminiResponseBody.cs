namespace AppGlobal.Models.Gemini;

// Root myDeserializedClass = JsonConvert.DeserializeObject<GeminiResponseBody>(myJsonResponse);
public class GeminiResponseBody
{
    public List<Candidate> candidates { get; set; } = [];
    public required UsageMetadata usageMetadata { get; set; }
    public string? modelVersion { get; set; }
}


public class Candidate
{
    public Content content { get; set; } = default!;
    public string finishReason { get; set; } = string.Empty;
    public int index { get; set; }
    public List<SafetyRating> safetyRatings { get; set; } = [];
}

public class Content
{
    public List<Part> parts { get; set; } = [];
    public string role { get; set; } = string.Empty;
}


public class SafetyRating
{
    public string category { get; set; } = string.Empty;
    public string probability { get; set; } = string.Empty;
}

public class UsageMetadata
{
    public int promptTokenCount { get; set; }
    public int candidatesTokenCount { get; set; }
    public int totalTokenCount { get; set; }
}

