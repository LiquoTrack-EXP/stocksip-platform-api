using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class AccountUsersControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountUsersControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory<Program>.TestToken);
    }

    [Fact]
    public async Task EmployeeCrud_ShouldWork()
    {
        // Arrange
        var accountId   = CustomWebApplicationFactory<Program>.TestAccount;
        var uniqueEmail = $"juan_{Guid.NewGuid():N}@local.com"; 

        var newEmployee = new
        {
            email       = uniqueEmail,
            password    = "Password123!",
            name        = "Juan Pérez",
            phoneNumber = "999888777",
            profileRole = "Admin",
            role        = "Admin"
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync(
            $"/api/v1/accounts/{accountId}/users", newEmployee);

        var responseBody = await createResponse.Content.ReadAsStringAsync();
        Assert.True(
            createResponse.IsSuccessStatusCode,
            $"Status: {createResponse.StatusCode} | Body: {responseBody}"
        );
    }
}