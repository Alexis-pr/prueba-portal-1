using Andromeda.API.DTOs.Auth;

namespace Andromeda.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request);
}