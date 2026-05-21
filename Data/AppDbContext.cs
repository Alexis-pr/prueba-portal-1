using Andromeda.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Andromeda.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Pqr> Pqrs => Set<Pqr>();
    public DbSet<PqrResponse> PqrResponses => Set<PqrResponse>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── USER ──────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            e.Property(u => u.Email).HasColumnName("email").IsRequired();
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).HasColumnName("password_hash");
            e.Property(u => u.GoogleId).HasColumnName("google_id");
            e.Property(u => u.FullName).HasColumnName("full_name").IsRequired();
            e.Property(u => u.Phone).HasColumnName("phone");
            e.Property(u => u.ProfilePhotoUrl).HasColumnName("profile_photo_url");
            e.Property(u => u.UserType).HasColumnName("user_type").HasDefaultValue("customer");
            e.Property(u => u.EmailVerified).HasColumnName("email_verified").HasDefaultValue(false);
            e.Property(u => u.LastLogin).HasColumnName("last_login");
            e.Property(u => u.CreatedAt).HasColumnName("created_at");
            e.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            e.Property(u => u.AuthProvider).HasColumnName("auth_provider").HasDefaultValue("local");
        });

        // ── FAVORITE ──────────────────────────────────────
        modelBuilder.Entity<Favorite>(e =>
        {
            e.ToTable("favorites");
            e.HasKey(f => f.Id);
            e.Property(f => f.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            e.Property(f => f.UserId).HasColumnName("customer_id");
            e.Property(f => f.EventId).HasColumnName("event_id");
            e.Property(f => f.CreatedAt).HasColumnName("created_at");

            e.HasOne(f => f.User)
             .WithMany(u => u.Favorites)
             .HasForeignKey(f => f.UserId);
        });

        // ── PQR ───────────────────────────────────────────
        modelBuilder.Entity<Pqr>(e =>
        {
            e.ToTable("pqrs");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            e.Property(p => p.UserId).HasColumnName("customer_id");
            e.Property(p => p.PqrNumber).HasColumnName("pqrs_number");
            e.Property(p => p.Category).HasColumnName("category");
            e.Property(p => p.Subject).HasColumnName("subject");
            e.Property(p => p.Body).HasColumnName("body");
            e.Property(p => p.Status).HasColumnName("status").HasDefaultValue("open");
            e.Property(p => p.ResolvedAt).HasColumnName("resolved_at");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(p => p.User)
             .WithMany(u => u.Pqrs)
             .HasForeignKey(p => p.UserId);
        });

        // ── PQR RESPONSE ──────────────────────────────────
        modelBuilder.Entity<PqrResponse>(e =>
        {
            e.ToTable("pqrs_responses");
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            e.Property(r => r.PqrId).HasColumnName("pqrs_id");
            e.Property(r => r.RespondedById).HasColumnName("responder_user_id");
            e.Property(r => r.Body).HasColumnName("body");
            e.Property(r => r.IsInternal).HasColumnName("is_internal").HasDefaultValue(false);
            e.Property(r => r.CreatedAt).HasColumnName("created_at");

            e.HasOne(r => r.Pqr)
             .WithMany(p => p.Responses)
             .HasForeignKey(r => r.PqrId);

            e.HasOne(r => r.RespondedBy)
             .WithMany()
             .HasForeignKey(r => r.RespondedById);
        });

        // ── ORDER (stub) ───────────────────────────────────
        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("orders");
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
            e.Property(o => o.UserId).HasColumnName("customer_id");

            e.HasOne(o => o.User)
             .WithMany(u => u.Orders)
             .HasForeignKey(o => o.UserId);
        });
    }
}