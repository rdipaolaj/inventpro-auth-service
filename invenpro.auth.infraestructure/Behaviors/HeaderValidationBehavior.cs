using invenpro.auth.common.Constants;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace invenpro.auth.infraestructure.Behaviors;

public class HeaderValidationBehavior<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor) : IPipelineBehavior<TRequest, TResponse> where TResponse : class, new()
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        dynamic response = new TResponse();

        foreach (var header in ApiConstants.PropertyHeaders)
        {
            if (!TryGetHeader(header, out var rawValue))
            {
                response.AddError(header, $"Header obligatorio {header}.");
                continue;
            }

            string finalValue = rawValue;
            switch (header)
            {
                case HeaderConstant.ChannelCode:
                    if (!ValidateChannelCode(rawValue, out var description, out var errorMsg))
                        response.AddError(header, errorMsg);
                    else
                        finalValue = description;
                    break;

                case HeaderConstant.TransactionId:
                case HeaderConstant.Timestamp:
                    if (!ValidatePattern(header, rawValue, out errorMsg))
                        response.AddError("E999999", errorMsg);
                    break;
            }

            _httpContextAccessor.HttpContext!.Items[header] = finalValue;

            Activity? currentActivity = Activity.Current;
            if (currentActivity != null)
            {
                currentActivity.SetTag("banbif.header." + header, finalValue);
                currentActivity.AddBaggage(header, finalValue);
            }
        }

        return response.IsValid() ? await next() : response;
    }

    private bool TryGetHeader(string name, out string? value)
    {
        if (_httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(name, out var vals))
        {
            value = vals.ToString();
            return true;
        }
        value = null;
        return false;
    }

    private bool ValidateChannelCode(string raw, out string description, out string error)
    {
        if (!raw.FindCode())
        {
            description = string.Empty;
            error = string.Format(MessageCode.InvalidChannelCodeTypeFormatMessage, raw);
            return false;
        }
        description = raw.GetDescripcion()!;
        error = string.Empty;
        return true;
    }

    private bool ValidatePattern(string header, string raw, out string error)
    {
        string? pattern = header switch
        {
            HeaderConstant.TransactionId => HeaderFormat.TransactionIdFormat,
            HeaderConstant.Timestamp => HeaderFormat.TimestampFormat,
            _ => null
        };

        if (pattern is null || Regex.IsMatch(raw, pattern))
        {
            error = string.Empty;
            return true;
        }

        error = $"El valor '{raw}' no cumple el formato para {header}.";
        return false;
    }
}