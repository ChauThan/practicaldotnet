using App.Application.Feature.Auth;
using App.Application.Repositories;
using App.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace App.Application.UnitTests.Features.Auth;

public class LogoutCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Logout.Handler _handler;

    public LogoutCommandHandlerTests()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        var userManager = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null, null, null, null, null, null, null, null
        );
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
        var options = new Mock<IOptions<IdentityOptions>>();
        var logger = new Mock<ILogger<SignInManager<ApplicationUser>>>();
        var schemes = new Mock<IAuthenticationSchemeProvider>();

        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            logger.Object,
            schemes.Object
        );

        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _handler = new Logout.Handler(_signInManagerMock.Object, _refreshTokenRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvalid_WhenRefreshTokenNotFound()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var invalidToken = "invalid-token";
        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(invalidToken, cancellationToken))
            .ReturnsAsync((App.Domain.RefreshToken?)null);
        var command = new Logout.Command { RefreshToken = invalidToken };

        // Act
        var response = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.False(response.Succeeded);
        Assert.Equal("Invalid or already revoked refresh token.", response.Message);
    }

    [Fact]
    public async Task Handle_ReturnsInvalid_WhenRefreshTokenRevoked()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var revokedToken = new App.Domain.RefreshToken
        {
            Token = "revoked-token",
            ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            RevokedDate = DateTime.UtcNow.AddMinutes(-1)
        };
        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(revokedToken.Token, cancellationToken))
            .ReturnsAsync(revokedToken);
        var command = new Logout.Command { RefreshToken = revokedToken.Token };

        // Act
        var response = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.False(response.Succeeded);
        Assert.Equal("Invalid or already revoked refresh token.", response.Message);
    }

    [Fact]
    public async Task Handle_ReturnsInvalid_WhenRefreshTokenExpired()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var expiredToken = new App.Domain.RefreshToken
        {
            Token = "expired-token",
            ExpiryDate = DateTime.UtcNow.AddMinutes(-1)
        };
        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(expiredToken.Token, cancellationToken))
            .ReturnsAsync(expiredToken);
        var command = new Logout.Command { RefreshToken = expiredToken.Token };

        // Act
        var response = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.False(response.Succeeded);
        Assert.Equal("Invalid or already revoked refresh token.", response.Message);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_AndRevokesToken_WhenValidToken()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var validToken = new App.Domain.RefreshToken
        {
            Token = "valid-token",
            ExpiryDate = DateTime.UtcNow.AddMinutes(10)
        };
        _refreshTokenRepositoryMock
            .Setup(r => r.GetByTokenAsync(validToken.Token, cancellationToken))
            .ReturnsAsync(validToken);
        _signInManagerMock.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);
        _refreshTokenRepositoryMock.Setup(r => r.RevokeTokenAsync(validToken.Token, cancellationToken)).Returns(Task.CompletedTask);
        var command = new Logout.Command { RefreshToken = validToken.Token };

        // Act
        var response = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(response.Succeeded);
        Assert.Equal("Logged out and token revoked.", response.Message);
        _signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);
        _refreshTokenRepositoryMock.Verify(r => r.RevokeTokenAsync(validToken.Token, cancellationToken), Times.Once);
    }
}
