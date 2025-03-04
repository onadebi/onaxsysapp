using AppGlobal.Services;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using System.Net;
using WebApp.Helpers.Filters;

namespace WebApp.Controllers.Api;

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
    [AuthAttributeDynamicOnx(nameof(UploadImage))]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        var thumbnailPath = await AppGlobal.Services.Media.ImageHelper.CreateThumbnailAsync(file, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot"));
        GenResponse<string> objResp = await fileManagerSvc.UploadFileToAzBlobAsync(file);
        return StatusCode(objResp.StatCode, objResp);
    }

    #region HELPERS
    #endregion
}
