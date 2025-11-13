using UserAuth.Domain.Entities;

namespace UserAuth.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
