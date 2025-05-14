using System.Security.Claims;
using Cloudot.Infrastructure.Auth.Constants;
using Cloudot.Module.Management.Application.Dtos;
using Cloudot.Module.Management.Application.Services;
using Cloudot.Shared.Results;
using Cloudot.WebAPI.Controllers.Management;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using IResult = Cloudot.Shared.Results.IResult;

namespace Cloudot.Module.Management.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }
    
    [Fact]
    public async Task RefreshToken_Should_Return_Success_Result()
    {
        // Arrange
        string refreshToken = "testtoken";
        var response = new UserSignInResponse
        {
            Email = "test@domain.com",
            RefreshToken = "newtoken",
            Expiration = DateTime.UtcNow.AddHours(8)
        };

        _authServiceMock
            .Setup(x => x.RefreshTokenAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(DataResult<UserSignInResponse>.Success(response, "Token yenilendi"));

        // Act
        var result = await _controller.RefreshToken(refreshToken, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        var data = Assert.IsAssignableFrom<IDataResult<UserSignInResponse>>(result.Value);
        Assert.True(data.IsSuccess);
        Assert.Equal("Token yenilendi", data.Message);
    }

    [Fact]
    public async Task Logout_Should_Return_Success()
    {
        // Arrange
        string refreshToken = "refresh123";
        Guid userId = Guid.NewGuid();

        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(AuthClaimTypes.UserId, userId.ToString())
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _authServiceMock
            .Setup(x => x.LogoutAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("Oturum sonlandırıldı."));

        // Act
        var result = await _controller.Logout(refreshToken, CancellationToken.None) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result!.StatusCode);
        var data = Assert.IsAssignableFrom<IResult>(result.Value);
        Assert.True(data.IsSuccess);
    }

}
