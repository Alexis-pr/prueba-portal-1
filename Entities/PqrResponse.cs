namespace Andromeda.API.Entities;

public class PqrResponse
{
    public Guid Id { get; set; }
    public Guid PqrId { get; set; }
    public Guid RespondedById { get; set; }
    public string Body { get; set; } = null!;
    public bool IsInternal { get; set; } = false;  // true = nota interna, cliente no la ve
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public Pqr Pqr { get; set; } = null!;
    public User RespondedBy { get; set; } = null!;
}