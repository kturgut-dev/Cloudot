using AutoMapper;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Dtos.Tenant;
using Cloudot.Module.Management.Application.Dtos.User;
using Cloudot.Module.Management.Domain.Tenant;
using Cloudot.Module.Management.Domain.User;

namespace Cloudot.Module.Management.Application;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserSignInDto>().ReverseMap();
        CreateMap<Tenant, TenantCreateDto>().ReverseMap();
    }
}
