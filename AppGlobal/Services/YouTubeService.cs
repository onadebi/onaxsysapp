using AutoMapper;
using AppGlobal.Config;
using AppGlobal.Models.YouTube;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using System.Text.Json;
using System.Web;
using System.Runtime.CompilerServices;

namespace AppGlobal.Services;
public class YouTubeService: IYouTubeService
{
    private readonly IHttpClientFactory _clienFactory;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;

    public YouTubeService(IHttpClientFactory client
        , IOptions<AppSettings> appSettings, IMapper mapper)
    {
        _clienFactory = client;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    public async Task<GenResponse<YouTubeApiResponse?>> YouTubeApiQueryRequest(YouTubeQueryRequest model, CancellationToken ct = default, [CallerMemberName] string callerName = "")
    {
        GenResponse<YouTubeApiResponse?> objResp = new();
        try
        {
            var _client = _clienFactory.CreateClient(nameof(_appSettings.ExternalAPIs.YoutubeApi));
            _client.Timeout = TimeSpan.FromMinutes(5);
            var payload = new YouTueRequestParams()
            {
                q = model.q,
                key = _appSettings.ExternalAPIs.YoutubeApi.YoutubeApiKey
            };
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var prop in payload.GetType().GetProperties())
            {
                var value = prop.GetValue(payload)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    query[prop.Name] = @value;
                }
            }
            HttpResponseMessage response = await _client.GetAsync($"?{query}", ct);
            if (response.IsSuccessStatusCode)
            {
                var resp = await response.Content.ReadAsStringAsync(ct);
                objResp.Result = !string.IsNullOrWhiteSpace(resp) ? JsonSerializer.Deserialize<YouTubeApiResponse>(resp) : null;
                objResp.StatCode = (int)StatusCodeEnum.OK;
                objResp.IsSuccess = true;
            }
            else
            {
                objResp.IsSuccess = false;
                objResp.Result = null;
                objResp.StatCode = (int)StatusCodeEnum.BadRequest;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(callerName, ex);
        }
        return objResp;
    }
}


public interface IYouTubeService
{
    Task<GenResponse<YouTubeApiResponse?>> YouTubeApiQueryRequest(YouTubeQueryRequest model, CancellationToken ct = default, [CallerMemberName] string callerName = "");
}