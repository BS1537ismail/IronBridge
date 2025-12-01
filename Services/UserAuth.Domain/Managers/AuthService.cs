using IronBridge.Shared.DTOs;
using IronBridge.Shared.Interfaces;
using UserAuth.Domain.Models;
using UserAuth.Domain.Interfaces;
using UserAuth.Repository.Interfaces;
using Mapster;

namespace UserAuth.Domain.Managers;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ICacheService _cacheService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, ICacheService cacheService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _cacheService = cacheService;
    }

    public async Task<LoginResponseDto?> RegisterAsync(RegisterUserDto dto)
    {
        // Check if user already exists
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
        {
            return null;
        }

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Role = dto.Role,
            IsActive = true
        };

        await _userRepository.AddAsync(user.Adapt<Repository.Data.User>());

        var token = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);

        if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return null;
        }

        var token = _jwtService.GenerateToken(user.Adapt<User>());

        return new LoginResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Token = token
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var cacheKey = $"user_{userId}";
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);

        if (cachedUser != null)
            return cachedUser;

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return null;

        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsActive = user.IsActive
        };

        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(10));
        return userDto;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var cacheKey = $"user_email_{email}";
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);

        if (cachedUser != null)
            return cachedUser;

        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return null;

        var userDto = new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsActive = user.IsActive
        };

        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(10));
        return userDto;
    }
}
