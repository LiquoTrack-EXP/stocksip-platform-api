using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class ProductControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ShouldWork()
    {
        // Arrange
        var accountId = "acc_001";
        var token = "mock_token";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("Ron Cartavio"), "Name");
        content.Add(new StringContent("Rum"), "Type");
        content.Add(new StringContent("Cartavio"), "Brand");
        content.Add(new StringContent("25.50"), "UnitPrice");
        content.Add(new StringContent("RC001"), "Code");
        content.Add(new StringContent("10"), "MinimumStock");
        content.Add(new StringContent("750"), "Content");

        // Act
        var response = await _client.PostAsync($"/api/v1/accounts/{accountId}/products", content);
        
        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK);
    }
}
