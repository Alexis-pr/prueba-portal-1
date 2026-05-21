namespace Andromeda.API.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    // Navegación
    public User User { get; set; } = null!;
}