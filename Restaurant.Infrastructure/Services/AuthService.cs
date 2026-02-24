using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Restaurant.Application.Common;
using Restaurant.Application.DTOs.AuthDtos;
using Restaurant.Application.Interfaces;
using Restaurant.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
                return ApiResponse<AuthResponseDto>.FailResponse("İstifadəçi tapılmadı.");

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!result)
                return ApiResponse<AuthResponseDto>.FailResponse("Şifrə yanlışdır.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddHours(1),
                UserId = user.Id,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? ""
            }, "Giriş uğurlu.");
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUser != null)
                return ApiResponse<AuthResponseDto>.FailResponse("Bu istifadəçi adı artıq mövcuddur.");

            var existingEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingEmail != null)
                return ApiResponse<AuthResponseDto>.FailResponse("Bu email artıq mövcuddur.");

            var user = new AppUser
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return ApiResponse<AuthResponseDto>.FailResponse(string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                await _roleManager.CreateAsync(new AppRole(dto.Role));

            await _userManager.AddToRoleAsync(user, dto.Role);

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.UtcNow.AddHours(1),
                UserId = user.Id,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? ""
            }, "Qeydiyyat uğurlu.");
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.Token);
            if (principal == null)
                return ApiResponse<AuthResponseDto>.FailResponse("Token yanlışdır.");

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return ApiResponse<AuthResponseDto>.FailResponse("Token yanlışdır.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return ApiResponse<AuthResponseDto>.FailResponse("Refresh token etibarsızdır.");

            var roles = await _userManager.GetRolesAsync(user);
            var newToken = GenerateJwtToken(user, roles);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return ApiResponse<AuthResponseDto>.SuccessResponse(new AuthResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                Expiration = DateTime.UtcNow.AddHours(1),
                UserId = user.Id,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? ""
            });
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Role = roles.FirstOrDefault() ?? ""
                });
            }

            return ApiResponse<List<UserDto>>.SuccessResponse(userDtos);
        }
        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new("FullName", user.FullName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!))
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

    }
}
