using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class ProductControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory<Program>.TestToken);
    }

    [Fact]
    public async Task CreateProduct_ShouldWork()
    {
        // Arrange
        var accountId   = CustomWebApplicationFactory<Program>.TestAccount;
        var uniqueName  = $"Ron Cartavio {Guid.NewGuid().ToString("N")[..6]}"; 

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(uniqueName),  "Name");
        content.Add(new StringContent("Rums"),      "Type");
        content.Add(new StringContent("Cartavio"),  "Brand");
        content.Add(new StringContent("25.50"),     "UnitPrice");
        content.Add(new StringContent("PEN"),       "Code");
        content.Add(new StringContent("10"),        "MinimumStock");
        content.Add(new StringContent("750"),       "Content");

        // Act
        var response = await _client.PostAsync($"/api/v1/accounts/{accountId}/products", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Status: {response.StatusCode} | Body: {responseBody}"
        );
    }
}