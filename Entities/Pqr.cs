namespace Andromeda.API.Entities;

public class Pqr
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PqrNumber { get; set; } = null!;   // Ej: PQR-2024-001
    public string Category { get; set; } = null!;    // pregunta/queja/reclamo/sugerencia
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
    public string Status { get; set; } = "open";     // open/in_progress/resolved/closed
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public User User { get; set; } = null!;
    public ICollection<PqrResponse> Responses { get; set; } = [];
}