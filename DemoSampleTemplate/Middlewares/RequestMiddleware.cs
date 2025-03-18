using DemoSampleTemplate.Core.Exceptions.HttpRequests;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DemoSampleTemplate.Service
{
    public class RequestMiddleware : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var response = exception switch
            {
                BaseRequestException apiEx => new ProblemDetails
                {
                    Status = (int)apiEx.StatusCode,
                    Type = apiEx.GetType().Name,
                    Title = apiEx.Message,
                    Detail = apiEx.Message,
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = exception.GetType().Name,
                    Title = "Internal server error",
                    Detail = "Internal server error",
                    Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
                }
            };

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}
