using invenpro.auth.common.Constants;
using invenpro.auth.common.Decorators;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace invenpro.auth.infraestructure.PreProcessors;

public class CaptureS3InfoPreProcessor<TRequest>(IHttpContextAccessor httpContextAccessor) : IRequestPreProcessor<TRequest>
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);

        if (requestType.GetCustomAttribute<TypologyAttribute>() is TypologyAttribute typologyAttribute)
        {
            _httpContextAccessor.HttpContext.Items[S3Context.Typology] = typologyAttribute.Type;
        }

        if (requestType.GetCustomAttribute<FunctionalityAttribute>() is FunctionalityAttribute functionalityAttribute)
        {
            _httpContextAccessor.HttpContext.Items[S3Context.Functionality] = functionalityAttribute.Type;
        }

        string backendCode = $"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}{_httpContextAccessor.HttpContext?.Request.Path}";

        _httpContextAccessor.HttpContext.Items[S3Context.BackendCode] = backendCode;

        return Task.CompletedTask;
    }
}