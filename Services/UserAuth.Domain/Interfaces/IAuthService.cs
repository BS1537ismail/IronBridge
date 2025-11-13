using IronBridge.Shared.DTOs;

namespace UserAuth.Domain.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> RegisterAsync(RegisterUserDto dto);
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
}
