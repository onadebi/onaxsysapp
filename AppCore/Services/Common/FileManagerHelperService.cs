using Azure.Storage.Blobs;
using AppGlobal.Config;
using Microsoft.Extensions.Options;
using OnaxTools.Dto.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace AppGlobal.Services;


public class FileManagerHelperService : IFileManagerHelperService
{
    private readonly ILogger<FileManagerHelperService> _logger;
    private readonly AppSettings _appsettings;

    public FileManagerHelperService(ILogger<FileManagerHelperService> logger
    , IOptions<AppSettings> appsettings)
    {
        _logger = logger;
        _appsettings = appsettings.Value;
    }

    public async Task<GenResponse<string>> UploadSingleFileToAzBlobStorage(IFormFile file, string path = "", CancellationToken ct = default!)
    {
        GenResponse<string> objResp = new() { IsSuccess = false };

        if (string.IsNullOrWhiteSpace(path))
        {
            path = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
        }
        try
        {
            if (!Directory.Exists(path))
            {
                DirectoryInfo info = Directory.CreateDirectory(path);
            }
            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            path = Path.Join(path, fileName);
            using (Stream fileStream = new FileStream(path, FileMode.Create))
            { await file.CopyToAsync(fileStream, ct); }

            #region AzureBlog storage
            var containerName = _appsettings.AzureBlobConfig?.DefaultContainerName;
            var blobConstring = _appsettings.AzureBlobConfig?.BlobStorageConstring;
            var blobSvcClient = new BlobServiceClient(blobConstring);

            var containerClient = blobSvcClient.GetBlobContainerClient(containerName);
            var blob = containerClient.GetBlobClient(Path.Combine("course_images", fileName));

            using Stream stream = new FileStream(path, FileMode.Open);
            var saveResponse = await blob.UploadAsync(stream, ct);
            if (saveResponse.GetRawResponse().Status != 201)
            {
                OnaxTools.Logger.LogInfo($"[UploadSingleFileToAzBlobStorage][{nameof(UploadSingleFileToAzBlobStorage)}] Error uploading document {file.Name} to container {containerName}");
            }
            else
            {
                objResp.IsSuccess = true;
                objResp.Message = $"{fileName}|{saveResponse.GetRawResponse().ClientRequestId}";
                objResp.Result = $"{fileName}|{AppBlobCloudFilePath(blob.Uri.AbsoluteUri)}";
                OnaxTools.Logger.LogInfo($"[UploadSingleFileToAzBlobStorage][{nameof(UploadSingleFileToAzBlobStorage)}] Uploaded successfully {saveResponse.Value.BlobSequenceNumber}!");
            }
            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return GenResponse<string>.Failed($"ERROR: {ex.Message}");
        }
        return objResp;
    }


    public async Task<GenResponse<string>> UploadFileToAzBlobAsync(IFormFile file, string path = "", CancellationToken ct = default!)
    {
        GenResponse<string> objResp = new() { IsSuccess = false };

        if (file == null || file.Length == 0)
        {
            objResp.Message = objResp.Error = "File is null or empty.";
            objResp.IsSuccess = false;
            objResp.Result = string.Empty;
            objResp.StatCode = (int)StatusCodes.Status400BadRequest;
            return objResp;
        }

        try
        {
            #region AzureBlob storage
            var containerName = _appsettings.AzureBlobConfig?.DefaultContainerName;
            var blobConstring = _appsettings.AzureBlobConfig?.BlobStorageConstring;
            var blobSvcClient = new BlobServiceClient(blobConstring);

            var containerClient = blobSvcClient.GetBlobContainerClient(containerName);
            var blob = containerClient.GetBlobClient(Path.Combine(_appsettings.AppName, $"{Guid.NewGuid()}_{file.FileName}"));

            using Stream stream = file.OpenReadStream();
            var saveResponse = await blob.UploadAsync(stream, ct);
            if (saveResponse.GetRawResponse().Status != 201)
            {
                OnaxTools.Logger.LogInfo($"[UploadFileToAzBlobAsync][{nameof(UploadFileToAzBlobAsync)}] Error uploading document {file.Name} to container {containerName}");
            }
            else
            {
                objResp.IsSuccess = true;
                objResp.Message = $"{file.FileName}|{saveResponse.GetRawResponse().ClientRequestId}";
                objResp.Result = $"{file.FileName}|{AppBlobCloudFilePath(blob.Uri.AbsoluteUri)}";
                OnaxTools.Logger.LogInfo($"[UploadFileToAzBlobAsync][{nameof(UploadFileToAzBlobAsync)}] Uploaded successfully {saveResponse.Value.BlobSequenceNumber}!");
            }
            #endregion
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return GenResponse<string>.Failed($"ERROR: {ex.Message}");
        }
        return objResp;
    }

    public async Task<GenResponse<string>> DeleteSingleFileFromAzBlobStorage(string fileName, CancellationToken ct = default!)
    {
        GenResponse<string> objResp = new() { IsSuccess = false };
        try
        {

            var containerName = _appsettings.AzureBlobConfig?.DefaultContainerName;
            var blobConstring = _appsettings.AzureBlobConfig?.BlobStorageConstring;
            var blobSvcClient = new BlobServiceClient(blobConstring);

            BlobContainerClient blobContainerClient = blobSvcClient.GetBlobContainerClient(containerName);

            var blob = blobContainerClient.GetBlobClient($"{fileName}");

            var objResult = await blob.DeleteIfExistsAsync();
            if (objResult.GetRawResponse()?.Status != 202)
            {
                OnaxTools.Logger.LogInfo($"[DeleteSingleFileFromAzBlobStorage][{nameof(DeleteSingleFileFromAzBlobStorage)}] Error deleting document {fileName} from container {containerName}");
            }
            else
            {
                objResp.IsSuccess = true;
                objResp.Message = $"Succeessfully deleted file [{fileName}]";
                objResp.Result = objResult.GetRawResponse().ClientRequestId;
                OnaxTools.Logger.LogInfo($"[DeleteSingleFileFromAzBlobStorage][{nameof(DeleteSingleFileFromAzBlobStorage)}] Uploaded successfully {objResult.Value}!");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return GenResponse<string>.Failed($"ERROR: {ex.Message}");
        }
        return objResp;
    }

    #region HELPERS
    public string AppBlobCloudFilePath(string fileName) => $"{fileName}{_appsettings.AzureBlobConfig?.BlobReadAccessYear2099}";
    //$"{_appsettings.AzureBlobConfig?.BlobStoragePath}{_appsettings.AzureBlobConfig?.DefaultContainerName}/{fileName}{_appsettings.AzureBlobConfig?.BlobReadAccessYear2099}";
    #endregion

}


public interface IFileManagerHelperService
{
    Task<GenResponse<string>> UploadSingleFileToAzBlobStorage(IFormFile file, string path = "", CancellationToken ct = default!);
    Task<GenResponse<string>> UploadFileToAzBlobAsync(IFormFile file, string path = "", CancellationToken ct = default!);
    Task<GenResponse<string>> DeleteSingleFileFromAzBlobStorage(string fileName, CancellationToken ct = default!);
    string AppBlobCloudFilePath(string fileName);
}