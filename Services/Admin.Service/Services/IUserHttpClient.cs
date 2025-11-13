using IronBridge.Shared.DTOs;

namespace Admin.Service.Services;

public interface IUserHttpClient
{
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}
