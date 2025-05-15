using Cloudot.Module.Management.Domain.User;
using Cloudot.Shared.Repository;
using Cloudot.Shared.Results;
using Microsoft.Extensions.Logging;

namespace Cloudot.Module.Management.Infrastructure.Services;

public class UserService(
    IUserEfRepository efRepository,
    IUnitOfWork _unitOfWork,
    ILogger<UserService> _logger) : IUserService
{
    public async Task<IDataResult<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Kullanıcı getiriliyor. ID: {UserId}", id);

        User? user = await efRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Kullanıcı bulunamadı. ID: {UserId}", id);
            return DataResult<User>.Fail("Kullanıcı bulunamadı.");
        }

        return DataResult<User>.Success(user);
    }

    public async Task<IDataResult<List<User>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Tüm kullanıcılar listeleniyor.");
        List<User> users = await efRepository.GetListAsync(cancellationToken);
        return DataResult<List<User>>.Success(users);
    }

    public async Task<IResult> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Yeni kullanıcı ekleniyor: {Email}", user.Email);

        await efRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Kullanıcı başarıyla eklendi. ID: {UserId}", user.Id);
        return Result.Success("Kullanıcı eklendi.");
    }

    public async Task<IResult> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Kullanıcı güncelleniyor. ID: {UserId}", user.Id);

        bool updated = await efRepository.UpdateAsync(user, cancellationToken);
        if (!updated)
        {
            _logger.LogWarning("Kullanıcı güncellenemedi. ID: {UserId}", user.Id);
            return Result.Fail("Güncelleme başarısız.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Kullanıcı başarıyla güncellendi. ID: {UserId}", user.Id);
        return Result.Success("Kullanıcı güncellendi.");
    }

    public async Task<IResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Kullanıcı siliniyor. ID: {UserId}", id);

        bool deleted = await efRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            _logger.LogWarning("Kullanıcı silinemedi. ID: {UserId}", id);
            return Result.Fail("Silme işlemi başarısız.");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Kullanıcı başarıyla silindi. ID: {UserId}", id);
        return Result.Success("Kullanıcı silindi.");
    }
}
