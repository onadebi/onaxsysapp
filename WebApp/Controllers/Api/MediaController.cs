using AppCore.Domain.AppCore.Dto;
using AppGlobal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using System.Net;

namespace WebApp.Controllers.Api;

    [AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class MediaController : ControllerBase
{
    private readonly IFileManagerHelperService fileManagerSvc;

    public MediaController(IFileManagerHelperService fileManagerSvc)
    {
        this.fileManagerSvc = fileManagerSvc;
    }

    [HttpPost(nameof(UploadImage))]
    [ProducesResponseType(typeof(GenResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        GenResponse<string> objResp = await fileManagerSvc.UploadFileToAzBlobAsync(file);
        return StatusCode(objResp.StatCode, objResp);
    }

    #region HELPERS
    public record FileUploader(IFormFile FileUpload);
    #endregion
}
