using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Cloudot.Core.Utilities.Caching;
using Cloudot.Core.Utilities.Security.Sessions;
using Cloudot.Core.Utilities.Security.Tokens;
using Cloudot.Infrastructure.Auth;
using Cloudot.Infrastructure.Auth.Jwt;
using Cloudot.Infrastructure.Messaging.Email;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Dtos.User;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Module.Management.Domain.User;
using Cloudot.Module.Management.Infrastructure.EntityFramework;
using Cloudot.Module.Management.Infrastructure.Services;
using Cloudot.Shared.Exceptions;
using Cloudot.Shared.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cloudot.Module.Management.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserEfRepository> _userRepositoryMock = new();
    private readonly Mock<IUnitOfWork<ManagementDbContext>> _unitOfWorkMock = new();
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<ICacheManager> _cacheManagerMock = new();
    private readonly Mock<IRefreshTokenStore> _refreshTokenStoreMock = new();
    private readonly Mock<ISessionManager> _sessionManagerMock = new();
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Mock<IJwtTokenHelper> _jwtTokenHelperMock = new();
    private readonly Mock<IStringLocalizer<AuthService>> _stringLocalizer = new();
    private readonly Mock<IExceptionFactory> _exceptionFactory = new();

    private readonly AuthService _authService;


    public AuthServiceTests()
    {
        _authService = new AuthService(
            new Mock<ILogger<UserService>>().Object,
            _currentUserMock.Object,
            _jwtTokenHelperMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _emailSenderMock.Object,
            _cacheManagerMock.Object,
            _refreshTokenStoreMock.Object,
            _sessionManagerMock.Object,
            _httpContextAccessorMock.Object,
            _stringLocalizer.Object,
            _exceptionFactory.Object
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
        await Assert.ThrowsAsync<ValidationAppException>(() =>
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

    [Fact]
    public async Task RefreshTokenAsync_Should_Return_New_Tokens_When_Valid()
    {
        // Arrange
        Guid userId = Guid.CreateVersion7();
        string refreshToken = Guid.NewGuid().ToString("N");

        RefreshTokenInfo storedToken = new()
        {
            Token = refreshToken,
            UserId = userId,
            IpAddress = "127.0.0.1",
            UserAgent = "TestAgent",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Expiration = DateTime.UtcNow.AddDays(1)
        };

        User user = new()
        {
            Id = userId,
            Email = "user@example.com",
            IsActive = true,
            IsMailVerified = true
        };

        JwtTokenResponse newTokens = new()
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _refreshTokenStoreMock
            .Setup(x => x.GetAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedToken);

        _userRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user); // 🛠️ DÜZELTİLEN KISIM

        _jwtTokenHelperMock
            .Setup(x => x.CreateToken(userId.ToString(), user.Email, null))
            .Returns(newTokens);

        _refreshTokenStoreMock
            .Setup(x => x.StoreAsync(It.IsAny<RefreshTokenInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _refreshTokenStoreMock
            .Setup(x => x.RemoveAsync(refreshToken, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Email, result.Data.Email);
        Assert.Equal(newTokens.AccessToken, result.Data.AccessToken);
        Assert.Equal(newTokens.RefreshToken, result.Data.RefreshToken);
        Assert.Equal(newTokens.AccessTokenExpiresAt, result.Data.Expiration);
    }
}