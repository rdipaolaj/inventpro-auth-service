using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace invenpro.auth.common.Responses;

public class ApiResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            Type valueType = objectResult.Value?.GetType() ?? typeof(object);
            bool esApiResponse = valueType.IsGenericType
                                 && valueType.GetGenericTypeDefinition() == typeof(ApiResponse<>);

            if (esApiResponse)
            {
                dynamic apiResp = objectResult.Value!;

                int statusToUse = apiResp.HttpStatus
                                  ?? (apiResp.Meta.Result
                                        ? StatusCodes.Status200OK
                                        : StatusCodes.Status400BadRequest);

                context.Result = new ObjectResult(apiResp)
                {
                    StatusCode = statusToUse
                };
                await next();
                return;
            }

            int originalStatus = objectResult.StatusCode ?? StatusCodes.Status200OK;
            ApiResponse<object> apiResponse = new ApiResponse<object>();

            if (originalStatus >= 200 && originalStatus < 300)
            {
                if (objectResult.Value != null)
                {
                    apiResponse.SetData(objectResult.Value!);
                }
            }
            else
            {
                apiResponse.Meta.Result = false;
                apiResponse.AddError("HTTP_" + originalStatus, "Error HTTP " + originalStatus);
            }

            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = originalStatus
            };
            await next();
            return;
        }

        if (context.Result is NoContentResult)
        {
            ApiResponse<object> apiResponse = new ApiResponse<object>();
            context.Result = new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status204NoContent
            };
        }

        await next();
    }
}