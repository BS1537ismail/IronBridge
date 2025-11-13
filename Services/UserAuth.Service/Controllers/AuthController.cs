using IronBridge.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using UserAuth.Domain.Interfaces;

namespace UserAuth.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterUserDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (result == null)
            return BadRequest(new { message = "User with this email already exists" });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (result == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid userId)
    {
        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }

    [HttpGet("user/email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        var user = await _authService.GetUserByEmailAsync(email);

        if (user == null)
            return NotFound(new { message = "User not found" });

        return Ok(user);
    }
}
