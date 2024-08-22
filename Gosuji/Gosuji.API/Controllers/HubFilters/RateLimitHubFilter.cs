using Gosuji.API.Helpers;
using Gosuji.API.Services;
using Gosuji.Client.Helpers.HttpResponseHandler;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Net;

namespace Gosuji.API.Controllers.HubFilters
{
    public class RateLimitHubFilter : IHubFilter
    {
        private RateLimitLogger rateLimitLogger;

        private readonly ConcurrentDictionary<string, RateLimitInfo> rateLimits = new();


        public RateLimitHubFilter(RateLimitLogger rateLimitLogger)
        {
            this.rateLimitLogger = rateLimitLogger;
        }

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            HttpContext? httpContext = invocationContext.Context.GetHttpContext();
            string partitionKey = RateLimitSetup.GetPartitionKey(httpContext);

            if (!IsRateLimitExceeded(partitionKey))
            {
                return await next(invocationContext);
            }
            else
            {
                rateLimitLogger.LogViolation(httpContext, invocationContext);
                return new HubResponse(HttpStatusCode.TooManyRequests);
            }
        }

        private bool IsRateLimitExceeded(string partitionKey)
        {
            DateTime now = DateTime.UtcNow;
            bool isRateLimited = false;

            rateLimits.AddOrUpdate(partitionKey,
                _ => new RateLimitInfo { Count = 1, Timestamp = now }, // Add
                (_, rateLimitInfo) => // Update
                {
                    lock (rateLimitInfo)
                    {
                        if (now - rateLimitInfo.Timestamp < RateLimitSetup.HUB_WINDOW)
                        {
                            rateLimitInfo.Count++;
                            if (rateLimitInfo.Count > RateLimitSetup.HUB_PERMIT_LIMIT)
                            {
                                isRateLimited = true;
                            }
                        }
                        else
                        {
                            rateLimitInfo.Count = 1;
                            rateLimitInfo.Timestamp = now;
                        }
                    }
                    return rateLimitInfo;
                });

            return isRateLimited;
        }

        private class RateLimitInfo
        {
            public int Count { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
