using Andromeda.API.Entities;

namespace Andromeda.API.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}