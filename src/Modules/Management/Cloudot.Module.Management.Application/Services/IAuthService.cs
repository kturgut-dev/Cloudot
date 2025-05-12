using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Shared.Results;

namespace Cloudot.Module.Management.Application.Services;

public interface IAuthService
{
    Task<IResult> RequestOtpAsync(UserSignInDto userSignInDto);
    Task<IDataResult<UserSignInResponse>> VerifyOtpAndSignInAsync(UserVerifyOtpDto dto);
    Task<IResult> SignUpAsync(UserSignUpDto dto);
    Task<IResult> VerifySignUpOtpAsync(UserVerifyOtpDto dto);
}