using FluentValidation;
using FluentValidation.Results;
using invenpro.auth.common.Exceptions;
using invenpro.auth.common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace invenpro.auth.infraestructure.Behaviors;

public class ValidatorBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<ValidationFailure>? failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .ToList();

        if (failures.Count != 0)
        {
            string typeName = request.GetGenericTypeName();
            _logger.LogWarning("Errores encontrados para command - {CommandType},  Command: {@Command}", typeName, request);

            throw new CustomException($"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
        }

        return await next();
    }
}