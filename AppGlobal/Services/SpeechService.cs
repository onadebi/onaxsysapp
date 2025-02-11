using AppGlobal.Config;
using AppGlobal.Models.Speech;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using OnaxTools.Dto.Identity;
using OnaxTools.Enums.Http;

namespace AppGlobal.Services;

public class SpeechService : ISpeechService
{
    //private readonly string UploadsDir;
    private readonly AppSettings _appsettings;

    public SpeechService(IOptions<AppSettings> appsettings)
    {
        //UploadsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Public", "Uploads");
        _appsettings = appsettings.Value;
    }

    public GenResponse<Array> GetAllVoices()
    {
        return GenResponse<Array>.Success(SpeechVoicesDto.SpeechVoicesList.ToArray(), StatusCodeEnum.OK);
    }

    public async Task<GenResponse<string>> ConvertTextToSpeach(TextToSpeechDto model, AppUserIdentity? user = null)
    {
        var audioFile = $"{DateTime.Now:yyyyMMdd-hhmmss}_{Guid.NewGuid()}.mp3";
        string? objResp = null;
        try
        {
            var maxCharacters = model.clearText.Length > 1000 ? model.clearText.Substring(0, 1000) : model.clearText.Substring(0);

            model.clearText = user == null ? ( maxCharacters.Length > 500? maxCharacters[..500] : maxCharacters) : maxCharacters;
            var speechConfig = SpeechConfig.FromSubscription(subscriptionKey: _appsettings.SpeechSynthesis.SpeechKey, region: _appsettings.SpeechSynthesis.SpeechLocation);
            speechConfig.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio24Khz48KBitRateMonoMp3);

            speechConfig.SpeechSynthesisVoiceName = !SpeechVoicesDto.SpeechVoicesList.ContainsValue(model.voice) ? "en-US-JennyNeural" : model.voice;

            using var speechSynthesizer = new SpeechSynthesizer(speechConfig);
            var speechResult = await speechSynthesizer.SpeakTextAsync(model.clearText);
            OutputSpeechSynthesisResult(speechResult, model.clearText);

            #region AzureBlog storage
            var containerName = _appsettings.AzureBlobConfig.DefaultContainerName;
            var blobConstring = _appsettings.AzureBlobConfig.BlobStorageConstring;
            var blobSvcClient = new BlobServiceClient(blobConstring);

            var containerClient = blobSvcClient.GetBlobContainerClient(containerName);
            var blockBlobClient = containerClient.GetBlockBlobClient(audioFile);

            using MemoryStream audioStream = new MemoryStream(speechResult.AudioData);
            var saveResponse = await blockBlobClient.UploadAsync(audioStream);
            if (saveResponse.GetRawResponse().Status != 201)
            {
                OnaxTools.Logger.LogInfo($"[SpeechService][{nameof(ConvertTextToSpeach)}] Error uploading document {blockBlobClient.Name} to container {blockBlobClient.BlobContainerName}");
            }
            else
            {
                objResp = AppBlobCloudFilePath(audioFile);
                OnaxTools.Logger.LogInfo($"[SpeechService][{nameof(ConvertTextToSpeach)}] Uploaded successfully {saveResponse.Value.BlobSequenceNumber}!");
            }
            #endregion
        }
        catch (Exception ex)
        {
            OnaxTools.Logger.LogException(ex);
        }
        return objResp == null ? GenResponse<string>.Failed("Oops. Unable to process audio file. Kindly try again") : GenResponse<string>.Success(objResp);
    }


    #region HELPERS
    private string AppBlobCloudFilePath(string fileName) => $"{_appsettings.AzureBlobConfig.BlobStoragePath}{_appsettings.AzureBlobConfig.DefaultContainerName}/{fileName}{_appsettings.AzureBlobConfig.BlobReadAccessYear2099}";
    public static string CheckMakeDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    private void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Console.WriteLine($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }

    }
    #endregion

}


public interface ISpeechService
{
    GenResponse<Array> GetAllVoices();
    Task<GenResponse<string>> ConvertTextToSpeach(TextToSpeechDto model, AppUserIdentity? user = null);
}