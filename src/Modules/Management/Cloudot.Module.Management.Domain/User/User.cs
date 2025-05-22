using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.User;

public class User : BaseTimestampEntity, IUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsRoot { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsMailVerified { get; set; } = false;
    
    public virtual ICollection<Tenant.Tenant> Tenants { get; set; }
}