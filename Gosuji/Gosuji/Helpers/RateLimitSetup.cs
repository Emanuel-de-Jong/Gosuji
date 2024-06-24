using Gosuji.Client;
using Gosuji.Services;
using Microsoft.AspNetCore.RateLimiting;
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
                    return RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(context), partition => new()
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 0
                    });
                });

                options.AddPolicy(G.ControllerRateLimitPolicyName, context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(context), partition => new()
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 0
                    });
                });

                options.AddPolicy("rl5", context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(context), partition => new()
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 0
                    });
                });

                options.AddPolicy("rl1", context =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(GetPartitionKey(context), partition => new()
                    {
                        PermitLimit = 1,
                        Window = TimeSpan.FromSeconds(10),
                        QueueLimit = 0
                    });
                });

                options.OnRejected = OnRejected;
            });
        }

        private static string GetPartitionKey(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private static ValueTask OnRejected(OnRejectedContext context, CancellationToken cancellationToken)
        {
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
            {
                context.HttpContext.Response.Headers.RetryAfter =
                    ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            Task writeResponse = context.HttpContext.Response.WriteAsync("Too Many Requests");

            RateLimitLogger logger = context.HttpContext.RequestServices.GetRequiredService<RateLimitLogger>();
            logger.LogViolation(context.HttpContext);

            return new ValueTask(writeResponse);
        }
    }
}
