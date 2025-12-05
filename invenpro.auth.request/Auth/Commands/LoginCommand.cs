using invenpro.auth.common.Responses;
using invenpro.auth.dto.Auth;
using MediatR;

namespace invenpro.auth.request.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<ApiResponse<LoginResponse>>;