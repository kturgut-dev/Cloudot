using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Dtos.User;
using Cloudot.Shared.Results;

namespace Cloudot.Module.Management.Application.Services;

public interface IAuthService
{
    Task<IResult> RequestOtpAsync(UserSignInDto dto, CancellationToken cancellationToken = default);

    Task<IDataResult<UserSignInResponse>> VerifyOtpAndSignInAsync(UserVerifyOtpDto dto,
        CancellationToken cancellationToken = default);

    Task<IResult> SignUpAsync(UserSignUpDto dto, CancellationToken cancellationToken = default);
    Task<IResult> VerifySignUpOtpAsync(UserVerifyOtpDto dto, CancellationToken cancellationToken = default);

    Task<IDataResult<UserSignInResponse>> RefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken = default);

    Task<IResult> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);
}