using System.ComponentModel.DataAnnotations.Schema;

namespace Microb.ApiGateway.Persistence.Entities;

public class UserRole
{
    [ForeignKey(nameof(Entities.User.Id))]
    public Guid UserId { get; set; }
    public User User { get; set; }

    [ForeignKey(nameof(Entities.Role.Id))]
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
}
