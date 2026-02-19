using TabuAI.Domain.Entities;

namespace TabuAI.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
