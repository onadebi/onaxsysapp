using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnaxTools.Dto.Http;
using OnaxTools.Enums.Http;

namespace WebApp.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class BlogController : ControllerBase
{
    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var categories = new List<string> { "Technology", "Health", "Travel", "Food" };
        return Ok(GenResponse<List<string>>.Success(categories, StatusCodeEnum.OK));
    }
}
