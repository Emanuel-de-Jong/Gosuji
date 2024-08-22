using Gosuji.API.Helpers;
using Gosuji.API.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Gosuji.API.Controllers.HubFilters
{
    public class RateLimitHubFilter : IHubFilter
    {
        private const int PERMIT_LIMIT = 2;
        private static readonly TimeSpan WINDOW = TimeSpan.FromSeconds(10);

        private RateLimitLogger rateLimitLogger;

        private readonly ConcurrentDictionary<string, RateLimitInfo> rateLimits = new();


        public RateLimitHubFilter(RateLimitLogger rateLimitLogger)
        {
            this.rateLimitLogger = rateLimitLogger;
        }

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext context,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            HttpContext? httpContext = context.Context.GetHttpContext();
            string partitionKey = RateLimitSetup.GetPartitionKey(httpContext);

            if (!IsRateLimitExceeded(partitionKey))
            {
                return await next(context);
            }
            else
            {
                rateLimitLogger.LogViolation(httpContext, context);
                throw new HubException("Hub_RateLimit");
            }
        }

        private bool IsRateLimitExceeded(string partitionKey)
        {
            DateTime now = DateTime.UtcNow;

            if (rateLimits.TryGetValue(partitionKey, out RateLimitInfo? rateLimitInfo) &&
                now - rateLimitInfo.Timestamp < WINDOW)
            {
                rateLimitInfo.Count++;
                if (rateLimitInfo.Count > PERMIT_LIMIT)
                {
                    return true;
                }
            }
            else
            {
                rateLimits[partitionKey] = new RateLimitInfo { Count = 1, Timestamp = now };
            }

            return false;
        }

        private class RateLimitInfo
        {
            public int Count { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
