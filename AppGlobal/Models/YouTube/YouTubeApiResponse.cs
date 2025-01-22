namespace AppGlobal.Models.YouTube;

public class YouTubeApiResponse
{
    public string kind { get; set; } = string.Empty;
    public string etag { get; set; } = string.Empty;
    public string nextPageToken { get; set; } = string.Empty;
    public string regionCode { get; set; } = string.Empty;
    public PageInfo pageInfo { get; set; } = default!;
    public List<Item> items { get; set; } = [];
}
// YouTubeApiResponse myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Default
{
    public string url { get; set; } = string.Empty;
    public int width { get; set; }
    public int height { get; set; }
}

public class High
{
    public string url { get; set; } = string.Empty;
    public int width { get; set; }
    public int height { get; set; }
}

public class Id
{
    public string kind { get; set; } = string.Empty;
    public string videoId { get; set; } = string.Empty;
}

public class Item
{
    public string kind { get; set; } = string.Empty;
    public string etag { get; set; } = string.Empty;
    public Id id { get; set; } = default!;
    public Snippet snippet { get; set; } = default!;
}

public class Medium
{
    public string url { get; set; }= string.Empty;
    public int width { get; set; }
    public int height { get; set; }
}

public class PageInfo
{
    public int totalResults { get; set; }
    public int resultsPerPage { get; set; }
}


public class Snippet
{
    public DateTime publishedAt { get; set; }
    public string channelId { get; set; } = string.Empty;
    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
    public Thumbnails? thumbnails { get; set; }
    public string channelTitle { get; set; } = string.Empty;
    public string liveBroadcastContent { get; set; } = string.Empty;
    public DateTime publishTime { get; set; }
}

public class Thumbnails
{
    public Default @default { get; set; } = default!;
    public Medium? medium { get; set; }
    public High? high { get; set; }
}

