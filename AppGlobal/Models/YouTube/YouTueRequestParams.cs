namespace AppGlobal.Models.YouTube;

public class YouTueRequestParams: YouTubeQueryRequest
{
    public string part { get; set; } = "snippet";
    public required string key { get; set; } = string.Empty; //YouTube API Key
    public string type { get; set; } = "video";

    public string publishedAfter { get; set; } = DateTime.UtcNow.AddYears(-5).ToString("yyyy-MM-ddTHH:mm:ssZ");
}

public class YouTubeQueryRequest
{
    public required string q { get; set; }
    public int maxResults { get; set; } = 1;
}
