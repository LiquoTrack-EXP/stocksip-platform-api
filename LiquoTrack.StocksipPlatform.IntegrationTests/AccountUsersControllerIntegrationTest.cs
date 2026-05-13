using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class AccountUsersControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccountUsersControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task EmployeeCrud_ShouldWork()
    {
        // Arrange
        var accountId = "acc_001";
        var token = "mock_token";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var newEmployee = new 
        { 
            email = "juan@local.com", 
            password = "Password123!",
            firstName = "Juan",
            lastName = "Pérez",
            role = "Employee"
        };

        // Act
        // 1. Create
        var createResponse = await _client.PostAsJsonAsync($"/api/v1/accounts/{accountId}/users", newEmployee);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
    }
}
