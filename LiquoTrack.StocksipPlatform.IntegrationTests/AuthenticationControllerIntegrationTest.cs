using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class AuthenticationControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthenticationControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task RegisterAndLogin_ShouldReturnJwtToken()
    {
        // Arrange
        var registerData = new 
        { 
            email = "test@stocksip.com", 
            password = "Password123!", 
            name = "Test User",
            businessName = "Test Business",
            role = "Admin" // Using Admin as a valid role from EUserRoles
        };

        var loginData = new { email = "test@stocksip.com", password = "Password123!" };

        // Act
        // 1. Register
        var regResponse = await _client.PostAsJsonAsync("/api/v1/sign-up", registerData);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, regResponse.StatusCode);

        // Act
        // 2. Login
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/sign-in", loginData);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var body = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("token", out _));
    }
}
