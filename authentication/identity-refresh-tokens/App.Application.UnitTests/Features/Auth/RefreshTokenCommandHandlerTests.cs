using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using App.Application.Feature.Auth;
using App.Application.Repositories;
using App.Application.Services;
using App.Domain;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using DomainRefreshToken = App.Domain.RefreshToken;
using FeatureRefreshToken = App.Application.Feature.Auth.RefreshToken;

namespace App.Application.UnitTests.Features.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly FeatureRefreshToken.Handler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _handler = new FeatureRefreshToken.Handler(_userManagerMock.Object, _refreshTokenRepositoryMock.Object, _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsInvalid_WhenRefreshTokenNotFoundOrInactive()
    {
        // Arrange
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((DomainRefreshToken?)null);
        var command = new FeatureRefreshToken.Command { RefreshToken = "invalid", AccessToken = "any" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Invalid or expired refresh token.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsInvalid_WhenUserNotFound()
    {
        // Arrange
        var token = new DomainRefreshToken
        {
            Token = "valid",
            JwtId = "jwtid",
            UserId = Guid.NewGuid(),
            ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            CreationDate = DateTime.UtcNow.AddMinutes(-10)
        };
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(token.Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        _userManagerMock.Setup(u => u.FindByIdAsync(token.UserId.ToString())).ReturnsAsync((ApplicationUser?)null);
        var command = new FeatureRefreshToken.Command { RefreshToken = token.Token, AccessToken = "any" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("User not found.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsInvalid_WhenAccessTokenInvalidFormat()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user" };
        var token = new DomainRefreshToken
        {
            Token = "valid",
            JwtId = "jwtid",
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            CreationDate = DateTime.UtcNow.AddMinutes(-10)
        };
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(token.Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        _userManagerMock.Setup(u => u.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        _jwtServiceMock.Setup(j => j.ValidateToken(It.IsAny<string>())).Throws(new Exception("Invalid format"));
        var command = new FeatureRefreshToken.Command { RefreshToken = token.Token, AccessToken = "badtoken" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Invalid access token format.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsInvalid_WhenAccessTokenDoesNotMatchRefreshToken()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user" };
        var token = new DomainRefreshToken
        {
            Token = "valid",
            JwtId = "jwtid",
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            CreationDate = DateTime.UtcNow.AddMinutes(-10)
        };
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(token.Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        _userManagerMock.Setup(u => u.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        var principal = new ClaimsPrincipal();
        _jwtServiceMock.Setup(j => j.ValidateToken(It.IsAny<string>())).Returns(principal);
        _jwtServiceMock.Setup(j => j.ExtractSubAndJti(principal)).Returns(("wronguserid", "wrongjti"));
        var command = new FeatureRefreshToken.Command { RefreshToken = token.Token, AccessToken = "token" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Access token does not match refresh token.", result.Message);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_AndGeneratesNewTokens()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid(), UserName = "user" };
        var token = new DomainRefreshToken
        {
            Token = "valid",
            JwtId = "jwtid",
            UserId = user.Id,
            ExpiryDate = DateTime.UtcNow.AddMinutes(10),
            CreationDate = DateTime.UtcNow.AddMinutes(-10)
        };
        _refreshTokenRepositoryMock.Setup(r => r.GetByTokenAsync(token.Token, It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);
        _userManagerMock.Setup(u => u.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        var principal = new ClaimsPrincipal();
        _jwtServiceMock.Setup(j => j.ValidateToken(It.IsAny<string>())).Returns(principal);
        _jwtServiceMock.Setup(j => j.ExtractSubAndJti(principal)).Returns((user.Id.ToString(), token.JwtId));
        _refreshTokenRepositoryMock.Setup(r => r.UpdateAsync(token, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var newJwtId = Guid.NewGuid().ToString();
        var newAccessToken = "newaccesstoken";
        var newExpiration = DateTime.UtcNow.AddMinutes(30);
        _jwtServiceMock.Setup(j => j.GenerateJwtToken(user)).Returns((newAccessToken, newExpiration, newJwtId));
        var newRefreshToken = new DomainRefreshToken { Token = "newrefreshtoken", JwtId = newJwtId, UserId = user.Id, ExpiryDate = DateTime.UtcNow.AddDays(7), CreationDate = DateTime.UtcNow };
        _jwtServiceMock.Setup(j => j.GenerateRefreshToken(user, newJwtId)).Returns(newRefreshToken);
        _refreshTokenRepositoryMock.Setup(r => r.AddAsync(newRefreshToken, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var command = new FeatureRefreshToken.Command { RefreshToken = token.Token, AccessToken = "token" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("Token refreshed.", result.Message);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.UserName, result.UserName);
        Assert.Equal(newAccessToken, result.AccessToken);
        Assert.Equal(newExpiration, result.AccessTokenExpiration);
        Assert.Equal(newRefreshToken.Token, result.RefreshToken);
    }
}
