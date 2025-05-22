using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.User;

public interface IUser : ITimestampEntity
{
    string FirstName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    bool IsRoot { get; set; }
    bool IsActive { get; set; }
    bool IsMailVerified { get; set; }
    
    ICollection<Tenant.Tenant> Tenants { get; set; }
}