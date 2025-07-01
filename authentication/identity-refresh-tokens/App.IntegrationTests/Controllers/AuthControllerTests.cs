using System.Net.Http.Json;
using App.Application.Feature.Auth;
using App.IntegrationTests.Helpers;

namespace App.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<ApiWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(ApiWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<Login.Response> LoginUserAsync(string email, string password)
    {
        var query = new Login.Query { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/Auth/login", query);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Login.Response>();
        return result!;
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenValidRequest()
    {
        // Arrange
        var email = $"integration_{Guid.NewGuid()}@example.com";
        var command = new Register.Command
        {
            Email = email,
            Password = "Test@123",
            FirstName = "Test",
            LastName = "User"
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/register", command);
        var result = await response.Content.ReadFromJsonAsync<Register.Response>();
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result!.Succeeded);
        Assert.NotNull(result.UserId);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenValidCredentials()
    {
        // Arrange
        var email = "integration.test@example.com";
        var password = "Test@123";
        // Act
        var result = await LoginUserAsync(email, password);
        // Assert
        Assert.NotNull(result);
        Assert.True(result!.Succeeded);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
    {
        // Arrange
        var query = new Login.Query { Email = "notfound@example.com", Password = "wrongpass" };
        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", query);
        var result = await response.Content.ReadFromJsonAsync<Login.Response>();
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(result);
        Assert.False(result!.Succeeded);
    }

    [Fact]
    public async Task RefreshToken_ReturnsOk_WhenValidTokens()
    {
        // Arrange
        var email = "integration.test@example.com";
        var password = "Test@123";
        var loginResult = await LoginUserAsync(email, password);
        var refreshCommand = new RefreshToken.Command
        {
            RefreshToken = loginResult.RefreshToken,
            AccessToken = loginResult.AccessToken
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/refresh-token", refreshCommand);
        var result = await response.Content.ReadFromJsonAsync<RefreshToken.Response>();
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result!.Succeeded);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_ReturnsUnauthorized_WhenInvalidToken()
    {
        // Arrange
        var refreshCommand = new RefreshToken.Command
        {
            RefreshToken = "invalid-refresh-token",
            AccessToken = "invalid-access-token"
        };
        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/refresh-token", refreshCommand);
        var result = await response.Content.ReadFromJsonAsync<RefreshToken.Response>();
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotNull(result);
        Assert.False(result!.Succeeded);
    }
}