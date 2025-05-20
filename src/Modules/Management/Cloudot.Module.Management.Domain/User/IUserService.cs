using Cloudot.Shared.Results;

namespace Cloudot.Module.Management.Domain.User;

public interface IUserService
{
    Task<IDataResult<User>> GetByIdAsync(Ulid id, CancellationToken cancellationToken = default);
    Task<IDataResult<List<User>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<IResult> AddAsync(User user, CancellationToken cancellationToken = default);
    Task<IResult> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(Ulid id, CancellationToken cancellationToken = default);
}