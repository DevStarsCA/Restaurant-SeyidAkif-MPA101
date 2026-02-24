using Restaurant.Application.Common;
using Restaurant.Application.DTOs.AuthDtos;

namespace Restaurant.Application.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto dto);
    Task<ApiResponse<List<UserDto>>> GetAllUsersAsync();
}