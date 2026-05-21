using Andromeda.API.Data;
using Andromeda.API.DTOs.Auth;
using Andromeda.API.Entities;
using Andromeda.API.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;

namespace Andromeda.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext context, ITokenService tokenService, IConfiguration config)
    {
        _context = context;
        _tokenService = tokenService;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Verificar si el email ya existe
        var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (exists)
            throw new Exception("El correo ya está registrado.");

        var user = new User
        {
            Email = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            UserType = "customer",
            EmailVerified = false
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(user),
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower().Trim());

        if (user == null || user.PasswordHash == null)
            throw new Exception("Credenciales inválidas.");

        var validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!validPassword)
            throw new Exception("Credenciales inválidas.");

        // Actualizar último login
        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(user),
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType
        };
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthRequestDto request)
    {
        // Validar el token con Google
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_config["GoogleAuth:ClientId"]]
        };

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch
        {
            throw new Exception("Token de Google inválido.");
        }

        // Buscar si ya existe el usuario
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == payload.Email);

        if (user == null)
        {
            // Registrar nuevo usuario con Google
            user = new User
            {
                Email = payload.Email,
                FullName = payload.Name,
                GoogleId = payload.Subject,
                ProfilePhotoUrl = payload.Picture,
                UserType = "customer",
                EmailVerified = true,
                AuthProvider = "google"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(user),
            Email = user.Email,
            FullName = user.FullName,
            UserType = user.UserType
        };
    }
}