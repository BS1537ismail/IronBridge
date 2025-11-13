using UserAuth.Domain.Models;

namespace UserAuth.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
