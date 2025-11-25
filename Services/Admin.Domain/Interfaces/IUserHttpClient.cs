using IronBridge.Shared.DTOs;

namespace Admin.Domain.Interfaces;

public interface IUserHttpClient
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}
