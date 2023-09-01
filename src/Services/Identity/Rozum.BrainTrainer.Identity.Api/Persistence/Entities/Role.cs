using Microsoft.AspNetCore.Identity;

namespace Rozum.BrainTrainer.Identity.Api.Persistence.Entities;

public class Role : IdentityRole<Guid>
{
    public List<UserRole> UserRoles { get; set; }
}
