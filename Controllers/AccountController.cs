using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendanceManagementSystem.Services.Interfaces;
using AttendanceManagementSystem.Models.DTOs;
using System.Security.Claims;

namespace AttendanceManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        public IActionResult ChangePassword()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var username = User.FindFirstValue(ClaimTypes.Name);
            
            ViewBag.UserId = userId;
            ViewBag.Username = username;
            
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                model.UserId = userId;

                if (model.NewPassword != model.ConfirmPassword)
                {
                    ModelState.AddModelError("", "New password and confirm password do not match");
                    return View(model);
                }

                var result = await _authService.ChangePasswordAsync(model);

                if (result)
                {
                    TempData["SuccessMessage"] = "Password changed successfully!";
                    return RedirectToAction("Logout");
                }

                ModelState.AddModelError("", "Failed to change password");
                return View(model);
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("", "Current password is incorrect");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(model);
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                await _authService.LogoutAsync(userId);
            }
            catch { }

            // Clear cookies
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");

            // Clear session
            HttpContext.Session.Clear();

            return RedirectToPage("/Login");
        }
    }
}
