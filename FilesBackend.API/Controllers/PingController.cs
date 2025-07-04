using Microsoft.AspNetCore.Mvc;

namespace FilesBackend.Controllers;

[ApiController]
[Route("ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok("PONG");
    }
}