using System;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

namespace ELearningPlatform.API.Helpers
{
    public static class RateLimitHelper
    {
        public static RateLimitPartition<string> GetRateLimitPartition(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "global";
            return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            });
        }
    }
}
