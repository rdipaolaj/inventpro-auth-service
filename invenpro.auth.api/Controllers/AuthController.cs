using Asp.Versioning;
using invenpro.auth.common.Constants;
using invenpro.auth.common.Responses;
using invenpro.auth.domain.AggregatesModel.UserAggregate;
using invenpro.auth.dto.Auth;
using invenpro.auth.request.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace invenpro.auth.api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("auth-service/api/v{v:apiVersion}/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Inicia sesión.
    /// </summary>
    [HttpPost]
    [Route("login")]
    [MapToApiVersion(1)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromHeader(Name = "ChannelCode")] string channelCode, [FromHeader(Name = "Timestamp")] string timestamp, [FromHeader(Name = "TransactionId")] string transactionId, [FromBody] LoginRequest request)
    {
        LoginCommand command = new LoginCommand(request.Email, request.Password);
        ApiResponse<LoginResponse> response = await _mediator.Send(command);

        return Ok(response);
    }

    /// <summary>
    /// Cierra sesión.
    /// </summary>
    [Authorize]
    [HttpPost]
    [Route("logout")]
    [MapToApiVersion(1)]
    [ProducesResponseType(typeof(ApiResponse<LogoutResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LogoutResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<LogoutResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout([FromHeader(Name = "ChannelCode")] string channelCode, [FromHeader(Name = "Timestamp")] string timestamp, [FromHeader(Name = "TransactionId")] string transactionId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            ApiResponse<LogoutResponse> failResponse = new ApiResponse<LogoutResponse>();
            failResponse = failResponse.WithError(AuthMessageCode.MissingUserIdFromToken, AuthMessageDescription.MissingUserIdFromToken, StatusCodes.Status401Unauthorized);
            return StatusCode(failResponse.HttpStatus ?? StatusCodes.Status401Unauthorized, failResponse);
        }

        LogoutCommand command = new LogoutCommand(userId);
        ApiResponse<LogoutResponse> response = await _mediator.Send(command);
        return Ok(response);
    }
}