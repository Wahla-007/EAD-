using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AttendanceManagementSystem.Data;
using AttendanceManagementSystem.Data.Entities;
using AttendanceManagementSystem.Helpers;
using AttendanceManagementSystem.Models.DTOs;
using AttendanceManagementSystem.Services.Interfaces;

namespace AttendanceManagementSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AttendanceManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AttendanceManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, string ipAddress)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid credentials");

            user.LastLoginDate = DateTime.UtcNow;
            var token = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                FullName = user.FullName,
                IsFirstLogin = user.IsFirstLogin,
                ExpiresIn = 24 * 60 * 60 // 24 hours in seconds
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow);

            if (token == null || token.User == null || !token.User.IsActive)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var jwtToken = GenerateJwtToken(token.User);
            var newRefreshToken = GenerateRefreshToken();

            token.IsRevoked = true;
            var newTokenEntity = new RefreshToken
            {
                UserId = token.UserId,
                Token = newRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(newTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = jwtToken,
                RefreshToken = newRefreshToken,
                UserId = token.User.UserId,
                Username = token.User.Username,
                Role = token.User.Role,
                FullName = token.User.FullName,
                IsFirstLogin = token.User.IsFirstLogin,
                ExpiresIn = 24 * 60 * 60
            };
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            var user = await _context.Users.FindAsync(request.UserId);

            if (user == null || !PasswordHasher.VerifyPassword(request.OldPassword, user.PasswordHash, user.PasswordSalt))
                throw new UnauthorizedAccessException("Invalid current password");

            var (hash, salt) = PasswordHasher.HashPassword(request.NewPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.IsFirstLogin = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            var (hash, salt) = PasswordHasher.HashPassword(newPassword);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.IsFirstLogin = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "DefaultSecretKeyForDevelopment123456"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}