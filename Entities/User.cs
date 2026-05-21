namespace Andromeda.API.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string? PasswordHash { get; set; }      // Null si usa Google
    public string? GoogleId { get; set; }           // Null si usa email/password
    public string FullName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string UserType { get; set; } = "customer"; // customer / employee / admin
    public bool EmailVerified { get; set; } = false;
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public ICollection<Favorite> Favorites { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Pqr> Pqrs { get; set; } = [];
    public string AuthProvider { get; set; } = "google";
}