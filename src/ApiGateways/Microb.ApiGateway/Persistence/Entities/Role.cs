using Microsoft.AspNetCore.Identity;

namespace Microb.ApiGateway.Persistence.Entities;

public class Role : IdentityRole<Guid>
{
    public List<UserRole> UserRoles { get; set; }
}
