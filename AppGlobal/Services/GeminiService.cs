using AutoMapper;
using AppGlobal.Config;
using AppGlobal.Models.Gemini;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace AppGlobal.Services;
public class GeminiService : IGeminiService
{
    private readonly IHttpClientFactory _clienFactory;
    private readonly IMapper _mapper;
    private readonly AppSettings _appSettings;

    public GeminiService(IHttpClientFactory client
        , IOptions<AppSettings> appSettings, IMapper mapper)
    {
        _clienFactory = client;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    public async Task<GenResponse<GeminiResponseBody?>> GeminiQueryRequest(GeminiRequestBody model, CancellationToken ct = default, [CallerMemberName]string callerName = "")
    {
        GenResponse<GeminiResponseBody?> objResp = new();
        try
        {
            var _client = _clienFactory.CreateClient(nameof(_appSettings.ExternalAPIs.GeminiApi));
            _client.Timeout = TimeSpan.FromMinutes(5);
            var payload = new
            {
                model.contents,
                generationConfig = new GeminiGenerationConfig()
            };
            var payloadBody = JsonSerializer.Serialize(payload);
            HttpResponseMessage response = await _client.PostAsJsonAsync(string.Empty, payload, ct);
            if (response.IsSuccessStatusCode)
            {
                //TODO: Use concurrent dictionary to store chat history [GeminiRequestBody model in Db and list in Redis]
                string? resp = await response.Content.ReadAsStringAsync(ct);
                objResp.Result = !string.IsNullOrWhiteSpace(resp) ? JsonSerializer.Deserialize<GeminiResponseBody>(resp) : null;
                objResp.StatCode = (int)StatusCodeEnum.OK;
                objResp.IsSuccess = true;
                if (objResp.Result != null && (objResp.Result.usageMetadata.totalTokenCount >= payload.generationConfig.maxOutputTokens))
                {
                    // Alert Token usage exceeded
                   objResp.Message = ("Token Usage exceeded!");
                }
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
            objResp.IsSuccess = false;
            objResp.Error = objResp.Message = ex.Message;
            objResp.StatCode = (int)StatusCodeEnum.ServerError;
            objResp.Result = null;
        }
        return objResp;
    }

}

public interface IGeminiService
{
    Task<GenResponse<GeminiResponseBody?>> GeminiQueryRequest(GeminiRequestBody model, CancellationToken ct = default, [CallerMemberName] string callerName = "");
}
