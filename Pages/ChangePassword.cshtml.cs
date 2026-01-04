using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AttendanceManagementSystem.Services.Interfaces;
using AttendanceManagementSystem.Models.DTOs;

namespace AttendanceManagementSystem.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IAuthService _authService;

        public ChangePasswordModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string OldPassword { get; set; } = string.Empty;

        [BindProperty]
        public string NewPassword { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(OldPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                ErrorMessage = "All fields are required";
                return Page();
            }

            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "New password and confirmation do not match";
                return Page();
            }

            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters long";
                return Page();
            }

            try
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
                if (userId == 0)
                {
                    return Redirect("/Login");
                }

                var request = new ChangePasswordRequestDto
                {
                    UserId = userId,
                    OldPassword = OldPassword,
                    NewPassword = NewPassword
                };

                await _authService.ChangePasswordAsync(request);
                
                SuccessMessage = "Password changed successfully! Redirecting to dashboard...";
                
                // Get role and redirect
                var role = HttpContext.Session.GetString("Role");
                TempData["RedirectUrl"] = GetDashboardUrl(role);
                TempData["PasswordChanged"] = "true";
                
                return Page();
            }
            catch (UnauthorizedAccessException ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while changing password";
                return Page();
            }
        }

        private string GetDashboardUrl(string? role)
        {
            return role switch
            {
                "Admin" => "/Admin/Dashboard",
                "Teacher" => "/Teacher/Dashboard",
                "Student" => "/Student/Dashboard",
                _ => "/Login"
            };
        }
    }
}
