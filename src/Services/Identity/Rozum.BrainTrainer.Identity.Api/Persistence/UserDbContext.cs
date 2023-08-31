using Microsoft.EntityFrameworkCore;

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
}
