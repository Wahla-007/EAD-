using AttendanceManagementSystem.Models.DTOs;

namespace AttendanceManagementSystem.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string ipAddress);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(int userId);
        Task<bool> ChangePasswordAsync(ChangePasswordRequestDto request);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
    }
}