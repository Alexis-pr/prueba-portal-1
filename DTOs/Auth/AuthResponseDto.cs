namespace Andromeda.API.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string UserType { get; set; } = null!;
}