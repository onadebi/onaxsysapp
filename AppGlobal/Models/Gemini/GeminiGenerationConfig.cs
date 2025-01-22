namespace AppGlobal.Models.Gemini;

public class GeminiGenerationConfig
{
    public int temperature = 1;
    public int topK { get; set; } = 64;
    public float topP { get; set; } = 0.95F;
    public int maxOutputTokens { get; set; } = 8192 * 3;
    public string responseMimeType { get; set; } = "application/json";
}


