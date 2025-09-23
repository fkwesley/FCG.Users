using FCG.Application.DTO.Auth;
using FCG.FiapCloudGames.Core.Entities;

namespace FCG.Application.Interfaces
{
    public interface IAuthService
    {
        LoginResponse GenerateToken(User user);
    }
}
