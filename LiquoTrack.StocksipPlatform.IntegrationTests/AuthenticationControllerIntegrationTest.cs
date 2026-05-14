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
        var uniqueId     = Guid.NewGuid().ToString("N")[..8];
        var email        = $"test_{uniqueId}@stocksip.com";
        var businessName = $"Test Business {uniqueId}";

        var registerData = new 
        { 
            email        = email,
            password     = "Password123!", 
            name         = "Test User",
            businessName = businessName,
            role         = "LiquorStoreOwner" 
        };

        var loginData = new 
        { 
            email    = email,
            password = "Password123!" 
        };

        // Act 
        var regResponse = await _client.PostAsJsonAsync("/api/v1/sign-up", registerData);
        var regBody     = await regResponse.Content.ReadAsStringAsync();
        Assert.True(
            regResponse.IsSuccessStatusCode,
            $"Register failed — Status: {regResponse.StatusCode} | Body: {regBody}"
        );

        // Act
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/sign-in", loginData);
        var loginBody     = await loginResponse.Content.ReadAsStringAsync();
        Assert.True(
            loginResponse.IsSuccessStatusCode,
            $"Login failed — Status: {loginResponse.StatusCode} | Body: {loginBody}"
        );

        // Assert 
        var body = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.TryGetProperty("token", out _), "Response does not contain 'token'");
    }
}