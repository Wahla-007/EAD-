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
                
                // Clear the session cookie
                context.Response.Cookies.Delete(".AspNetCore.Session");
                context.Response.Cookies.Delete("AccessToken");
                
                // Redirect to login page
                context.Response.Redirect("/Login");
            }
        }
    }
}
