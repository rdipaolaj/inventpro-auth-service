using invenpro.auth.domain.AggregatesModel.UserAggregate;
using invenpro.auth.domain.Seedwork;
using invenpro.auth.repository.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace invenpro.auth.repository;

public class AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options, RepositoryTimingInterceptor timingInterceptor) : DbContext(options), IUnitOfWork
{
    private readonly RepositoryTimingInterceptor _timingInterceptor = timingInterceptor;
    public virtual DbSet<User> Users { get; set; } = null!;


    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        int changes = await base.SaveChangesAsync(cancellationToken);
        return changes >= 0;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(_timingInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.Email)
                  .IsUnique();

            entity.Property(x => x.Id)
                  .HasMaxLength(50);

            entity.Property(x => x.Email)
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(x => x.PasswordHash)
                  .HasMaxLength(255)
                  .IsRequired();

            entity.Property(x => x.Name)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(x => x.Avatar)
                  .HasMaxLength(500);

            entity.Property(x => x.CreatedAt)
                  .HasColumnType("datetime(6)");

            entity.Property(x => x.UpdatedAt)
                  .HasColumnType("datetime(6)");
        });
    }
}