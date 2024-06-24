using Gosuji.Client;
using Gosuji.Services;
using System.Globalization;
using System.Threading.RateLimiting;

namespace Gosuji.Helpers
{
    public class RateLimitSetup
    {
        public static void AddRateLimiters(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<RateLimitLogger>();

            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy(G.RazorRateLimitPolicyName, context =>
                {
                    string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, partition => new()
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromSeconds(10),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.AddPolicy(G.ControllerRateLimitPolicyName, context =>
                {
                    string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, partition => new()
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromSeconds(10),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync("Too Many Requests");

                    RateLimitLogger logger = context.HttpContext.RequestServices.GetRequiredService<RateLimitLogger>();
                    logger.LogViolation(context.HttpContext);
                };
            });
        }
    }
}
