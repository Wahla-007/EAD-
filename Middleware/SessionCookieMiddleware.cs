using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace AttendanceManagementSystem.Middleware
{
    public class SessionCookieMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SessionCookieMiddleware> _logger;

        public SessionCookieMiddleware(RequestDelegate next, ILogger<SessionCookieMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (CryptographicException ex)
            {
                _logger.LogWarning(ex, "Invalid session cookie detected. Clearing cookies and redirecting.");
                
                // Only handle if response hasn't started
                if (!context.Response.HasStarted)
                {
                    // Clear the session cookies
                    context.Response.Cookies.Delete(".AttendanceSystem.Session");
                    context.Response.Cookies.Delete("AccessToken");
                    
                    // Redirect to login page
                    context.Response.Redirect("/Login");
                }
            }
        }
    }
}
