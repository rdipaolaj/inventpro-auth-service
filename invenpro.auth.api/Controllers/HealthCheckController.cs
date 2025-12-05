using invenpro.auth.common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace invenpro.auth.api.Controllers;

[Route("auth-service/")]
[ApiController]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public ActionResult HealthCheck() => Ok("HealthCheck Auth Service OK");
}