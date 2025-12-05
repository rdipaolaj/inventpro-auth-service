using FluentValidation;
using FluentValidation.Results;
using invenpro.auth.common.Constants;
using invenpro.auth.common.Enums;
using invenpro.auth.common.Exceptions;
using invenpro.auth.common.Responses;
using Microsoft.AspNetCore.Diagnostics;

namespace invenpro.auth.api.Configuration;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ApiResponse<string> apiResponse = new();
        int statusCode;

        if (exception is CustomException customEx)
        {
            HandleCustom(customEx, apiResponse, out statusCode);
        }
        else
        {
            HandleUnknown(exception, apiResponse, out statusCode);
        }

        apiResponse.SetHttpStatus(statusCode);
        httpContext.Response.ContentType = MediaTypeConstant.ApplicationJson;
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);

        return true;
    }
    private static void HandleCustom(CustomException exception, ApiResponse<string> apiResponse, out int statusCode)
    {
        statusCode = StatusCodes.Status400BadRequest;

        if (exception.InnerException is ValidationException validationEx)
        {
            foreach (ValidationFailure failure in validationEx.Errors)
            {
                string codigo = string.IsNullOrWhiteSpace(failure.ErrorCode)
                    ? MessageCode.Error
                    : failure.ErrorCode;

                string mensaje = failure.ErrorMessage;

                apiResponse.AddMessage(codigo, mensaje, nameof(TipoMensaje.WARNING));
            }
            apiResponse.Meta.Result = false;
        }
        else
        {
            apiResponse.AddError(MessageCode.Error, exception.Message);
        }
    }

    private void HandleUnknown(Exception exception, ApiResponse<string> apiResponse, out int statusCode)
    {
        statusCode = StatusCodes.Status500InternalServerError;
        apiResponse.AddError(MessageCode.Error, MessageDescription.Error);
        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
    }
}