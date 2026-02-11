using System.Collections.Concurrent;

namespace Visitapp.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, (DateTime LastRequest, int Count)> _requests = new();
        private const int MaxRequests = 10; // 10 requests per minute
        private const int TimeWindowMinutes = 1;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply rate limiting to auth endpoints
            if (!context.Request.Path.StartsWithSegments("/api/auth") && 
                !context.Request.Path.StartsWithSegments("/api/v2/auth"))
            {
                await _next(context);
                return;
            }

            var clientIp = GetClientIpAddress(context);
            var now = DateTime.UtcNow;

            _requests.AddOrUpdate(clientIp, 
                (now, 1), 
                (key, value) =>
                {
                    if (now.Subtract(value.LastRequest).TotalMinutes > TimeWindowMinutes)
                    {
                        return (now, 1);
                    }
                    return (now, value.Count + 1);
                });

            if (_requests[clientIp].Count > MaxRequests)
            {
                _logger.LogWarning("Rate limit exceeded for IP: {ClientIp}", clientIp);
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                return;
            }

            await _next(context);
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            return context.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                   ?? context.Request.Headers["X-Real-IP"].FirstOrDefault() 
                   ?? context.Connection.RemoteIpAddress?.ToString() 
                   ?? "unknown";
        }
    }
}