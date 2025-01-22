namespace AppGlobal.Models.Gemini;

public class GeminiRequestBody
{
    public List<Contents> contents { get; set; } = [];

    public GeminiRequestBody Add(List<Contents> contents)
    {
        this.contents.AddRange(contents);
        return this;
    }
}
public class Contents
{
    public required string role { get; set; }
    public List<Part> parts { get; set; } = [];
}


public class Part
{
    public required string text { get; set; }
}
