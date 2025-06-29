using System.Net.Http.Headers;
using System.Net.Http.Json;
using App.Application.Feature.Roles;
using App.IntegrationTests.Helpers;
using App.Application.Feature.Auth;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace App.IntegrationTests.Controllers;

public class RolesControllerTests : IClassFixture<ApiWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory<Program> _factory;

    public RolesControllerTests(ApiWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> AuthenticateAsync(string email, string password)
    {
        var login = new Login.Query { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/Auth/login", login);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Login.Response>();
        return result!.AccessToken;
    }

    private void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreateRole_ReturnsOk_WhenValidRequest()
    {
        // Arrange
        var token = await AuthenticateAsync("integration.test@example.com", "Test@123");
        SetAuthHeader(token);
        var command = new CreateRole.Command { RoleName = $"TestRole_{Guid.NewGuid()}" };
        // Act
        var response = await _client.PostAsJsonAsync("/api/Roles/create", command);
        var result = await response.Content.ReadFromJsonAsync<CreateRole.Response>();
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result!.Succeeded);
        Assert.NotNull(result.RoleId);
    }

    [Fact]
    public async Task CreateRole_ReturnsBadRequest_WhenRoleExists()
    {
        // Arrange
        var token = await AuthenticateAsync("integration.test@example.com", "Test@123");
        SetAuthHeader(token);
        var roleName = $"TestRole_{Guid.NewGuid()}";
        var command = new CreateRole.Command { RoleName = roleName };
        await _client.PostAsJsonAsync("/api/Roles/create", command); // create once
        // Act
        var response = await _client.PostAsJsonAsync("/api/Roles/create", command); // create again
        var result = await response.Content.ReadFromJsonAsync<CreateRole.Response>();
        // Assert
        Assert.False(result!.Succeeded);
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result.Errors);
    }

    [Fact]
    public async Task CreateRole_ReturnsUnauthorized_WhenNoToken()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var command = new CreateRole.Command { RoleName = $"TestRole_{Guid.NewGuid()}" };
        // Act
        var response = await _client.PostAsJsonAsync("/api/Roles/create", command);
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AssignRoleToUser_ReturnsOk_WhenValidRequest()
    {
        // Arrange
        var token = await AuthenticateAsync("integration.test@example.com", "Test@123");
        SetAuthHeader(token);
        var roleName = $"TestRole_{Guid.NewGuid()}";
        // Create role
        var createRoleCmd = new CreateRole.Command { RoleName = roleName };
        var createRoleResp = await _client.PostAsJsonAsync("/api/Roles/create", createRoleCmd);
        var createRoleResult = await createRoleResp.Content.ReadFromJsonAsync<CreateRole.Response>();
        Assert.True(createRoleResult!.Succeeded);
        // Assign role
        var userId = createRoleResult.RoleId.ToString(); // Actually, need a valid userId
        var login = await AuthenticateAsync("integration.test@example.com", "Test@123");
        var userInfo = await _client.PostAsJsonAsync("/api/Auth/login", new Login.Query { Email = "integration.test@example.com", Password = "Test@123" });
        var userResult = await userInfo.Content.ReadFromJsonAsync<Login.Response>();
        var assignCmd = new AssignRoleToUser.Command { UserId = userResult!.UserId.ToString()!, RoleName = roleName };
        var response = await _client.PostAsJsonAsync("/api/Roles/assign-to-user", assignCmd);
        var result = await response.Content.ReadFromJsonAsync<AssignRoleToUser.Response>();
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result!.Succeeded);
        Assert.Equal(roleName, result.RoleName);
    }

    [Fact]
    public async Task AssignRoleToUser_ReturnsBadRequest_WhenRoleDoesNotExist()
    {
        // Arrange
        var token = await AuthenticateAsync("integration.test@example.com", "Test@123");
        SetAuthHeader(token);
        var assignCmd = new AssignRoleToUser.Command { UserId = "00000000-0000-0000-0000-000000000001", RoleName = $"NotExistRole_{Guid.NewGuid()}" };
        var response = await _client.PostAsJsonAsync("/api/Roles/assign-to-user", assignCmd);
        var result = await response.Content.ReadFromJsonAsync<AssignRoleToUser.Response>();
        // Assert
        Assert.False(result!.Succeeded);
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result.Errors);
    }

    [Fact]
    public async Task AssignRoleToUser_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        // Arrange
        var token = await AuthenticateAsync("integration.test@example.com", "Test@123");
        SetAuthHeader(token);
        var roleName = $"TestRole_{Guid.NewGuid()}";
        // Create role
        var createRoleCmd = new CreateRole.Command { RoleName = roleName };
        var createRoleResp = await _client.PostAsJsonAsync("/api/Roles/create", createRoleCmd);
        var createRoleResult = await createRoleResp.Content.ReadFromJsonAsync<CreateRole.Response>();
        Assert.True(createRoleResult!.Succeeded);
        // Assign role to non-existent user
        var assignCmd = new AssignRoleToUser.Command { UserId = "00000000-0000-0000-0000-000000000001", RoleName = roleName };
        var response = await _client.PostAsJsonAsync("/api/Roles/assign-to-user", assignCmd);
        var result = await response.Content.ReadFromJsonAsync<AssignRoleToUser.Response>();
        // Assert
        Assert.False(result!.Succeeded);
        Assert.False(response.IsSuccessStatusCode);
        Assert.NotNull(result.Errors);
    }

    [Fact]
    public async Task AssignRoleToUser_ReturnsUnauthorized_WhenNoToken()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;
        var assignCmd = new AssignRoleToUser.Command { UserId = "00000000-0000-0000-0000-000000000001", RoleName = $"TestRole_{Guid.NewGuid()}" };
        // Act
        var response = await _client.PostAsJsonAsync("/api/Roles/assign-to-user", assignCmd);
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
