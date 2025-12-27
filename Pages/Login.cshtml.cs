using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AttendanceManagementSystem.Services.Interfaces;
using AttendanceManagementSystem.Models.DTOs;

namespace AttendanceManagementSystem.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginModel(IAuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Check if already logged in
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var role = HttpContext.Session.GetString("Role");
                RedirectToDashboard(role);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

                var loginRequest = new LoginRequestDto
                {
                    Username = Username,
                    Password = Password
                };

                var result = await _authService.LoginAsync(loginRequest, ipAddress);

                // Store tokens in cookies
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(1)
                };

                Response.Cookies.Append("AccessToken", result.AccessToken, cookieOptions);
                Response.Cookies.Append("RefreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });

                // Store user info in session
                HttpContext.Session.SetString("UserId", result.UserId.ToString());
                HttpContext.Session.SetString("Username", result.Username);
                HttpContext.Session.SetString("Role", result.Role);
                HttpContext.Session.SetString("FullName", result.FullName);
                HttpContext.Session.SetString("IsFirstLogin", result.IsFirstLogin.ToString());

                // Check if first login (redirect to change password)
                if (result.IsFirstLogin)
                {
                    return RedirectToPage("/ChangePassword");
                }

                // Redirect based on role
                return RedirectToDashboard(result.Role);
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }

        private IActionResult RedirectToDashboard(string? role)
        {
            return role switch
            {
                "Admin" => Redirect("/Admin/Dashboard"),
                "Teacher" => Redirect("/Teacher/Dashboard"),
                "Student" => Redirect("/Student/Dashboard"),
                _ => RedirectToPage("/Login")
            };
        }
    }
}