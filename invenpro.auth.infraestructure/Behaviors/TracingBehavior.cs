using MediatR;
using System.Diagnostics;

namespace invenpro.auth.infraestructure.Behaviors;

public sealed class TracingBehavior<TRequest, TResponse>(ActivitySource activitySource) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ActivitySource _activitySource = activitySource;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Activity? activity = _activitySource.StartActivity(typeof(TRequest).Name, ActivityKind.Internal);
        try
        {
            if (activity is not null)
            {
                activity.SetTag("request.type", typeof(TRequest).FullName);
            }

            TResponse response = await next();
            return response;
        }
        finally
        {
            if (activity is not null)
            {
                activity.Dispose();
            }
        }
    }
}