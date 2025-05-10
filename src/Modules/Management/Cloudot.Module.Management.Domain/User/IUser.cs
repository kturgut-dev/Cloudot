using Cloudot.Shared.Entity;

namespace Cloudot.Module.Management.Domain.User;

public interface IUser : IAuditEntity
{
    string FirstName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    bool IsRoot { get; set; }
    bool IsActive { get; set; }
}