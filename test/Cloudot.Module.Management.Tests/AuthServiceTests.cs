using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Cloudot.Core.Utilities.Caching;
using Cloudot.Core.Utilities.Security.Sessions;
using Cloudot.Core.Utilities.Security.Tokens;
using Cloudot.Infrastructure.Messaging.Email;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Module.Management.Infrastructure.Services;
using Cloudot.Shared.Exceptions;
using Cloudot.Shared.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cloudot.Module.Management.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<ICacheManager> _cacheManagerMock = new();
    private readonly Mock<IRefreshTokenStore> _refreshTokenStoreMock = new();
    private readonly Mock<ISessionManager> _sessionManagerMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

    private readonly AuthService _authService;


    public AuthServiceTests()
    {
        _authService = new AuthService(
            new Mock<ILogger<UserService>>().Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _emailSenderMock.Object,
            _cacheManagerMock.Object,
            _refreshTokenStoreMock.Object,
            _sessionManagerMock.Object,
            _httpContextAccessorMock.Object
        );
    }

    /// <summary>
    /// E-posta formatı geçersizse ArgumentException fırlatılmalı
    /// </summary>
    [Fact]
    public async Task RequestOtpAsync_Should_Throw_When_Email_Invalid()
    {
        // Arrange: Geçersiz formatta bir e-posta adresi içeren DTO
        UserSignInDto dto = new() { Email = "gecersiz-email" };

        // Act & Assert: Hatalı e-posta formatı nedeniyle exception fırlatılması beklenir
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _authService.RequestOtpAsync(dto));
    }

    /// <summary>
    /// Geçerli e-posta girilmiş ama kullanıcı bulunamamışsa NotFoundAppException fırlatılmalı
    /// </summary>
    [Fact]
    public async Task RequestOtpAsync_Should_Throw_When_User_Not_Found()
    {
        // Arrange: Kullanıcı veritabanında yok
        UserSignInDto dto = new() { Email = "user@domain.com" };

        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
            .ReturnsAsync((User?)null);

        // Act & Assert: Kullanıcı bulunamadığında exception beklenir
        await Assert.ThrowsAsync<NotFoundAppException>(() =>
            _authService.RequestOtpAsync(dto));
    }

    /// <summary>
    /// Kullanıcı pasifse UnauthorizedAppException fırlatılmalı
    /// </summary>
    [Fact]
    public async Task RequestOtpAsync_Should_Throw_When_User_Not_Active()
    {
        // Arrange: Pasif kullanıcıyı dönen repository mock'u
        UserSignInDto dto = new() { Email = "aktif-degil@domain.com" };

        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), default))
            .ReturnsAsync(new User { Email = dto.Email, IsActive = false });

        // Act & Assert: Aktif olmayan kullanıcı giriş yapamaz
        await Assert.ThrowsAsync<UnauthorizedAppException>(() =>
            _authService.RequestOtpAsync(dto));
    }

}