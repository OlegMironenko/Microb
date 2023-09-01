using Microsoft.EntityFrameworkCore;
using Rozum.BrainTrainer.Identity.Api.Extensions;
using Rozum.BrainTrainer.Identity.Api.Persistence.Entities;

namespace Rozum.BrainTrainer.Identity.Api.Persistence;

public class UserDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public UserDbContext(DbContextOptions<UserDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("UsersDatabase"));
        optionsBuilder.UseOpenIddict<Guid>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        // modelBuilder.Entity<UserRole>()
        //     .HasOne(ur => ur.User)
        //     .WithMany(ur => ur.UserRoles)
        //     .HasForeignKey(ur => ur.UserId);
        // modelBuilder.Entity<UserRole>()
        //     .HasOne(ur => ur.Role)
        //     .WithMany(ur => ur.UserRoles)
        //     .HasForeignKey(ur => ur.RoleId);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName().ToSnakeCase());

            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToSnakeCase());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToSnakeCase());
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
            }
        }
    }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<UserRole> UserRoles { get; set; }
}
