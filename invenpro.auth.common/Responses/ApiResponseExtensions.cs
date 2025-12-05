using Microsoft.AspNetCore.Http;

namespace invenpro.auth.common.Responses;

public static class ApiResponseExtensions
{
    public static ApiResponse<T> WithError<T>(this ApiResponse<T> resp, string code, string message, int statusCode = StatusCodes.Status400BadRequest)
    {
        resp.SetHttpStatus(statusCode);
        resp.AddError(code, message);
        return resp;
    }

    public static ApiResponse<T> WithSuccess<T>(this ApiResponse<T> resp, T data, int statusCode = StatusCodes.Status200OK)
    {
        resp.SetData(data);
        resp.AddSuccessMessage();
        resp.SetHttpStatus(statusCode);
        return resp;
    }
}