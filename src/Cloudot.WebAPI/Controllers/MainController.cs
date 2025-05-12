using Microsoft.AspNetCore.Mvc;

namespace Cloudot.WebAPI.Controllers;

public class MainController : ControllerBase
{

    /// <summary>
    /// Temel return methodu
    /// </summary>
    [NonAction]
    protected IActionResult ReturnResult(Cloudot.Shared.Results.IResult result)
    {
        return StatusCode(result.StatusCode, result);
    }
}