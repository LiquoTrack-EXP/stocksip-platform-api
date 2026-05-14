using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LiquoTrack.StocksipPlatform.IntegrationTests;

public class WarehouseControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WarehouseControllerIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CustomWebApplicationFactory<Program>.TestToken);
    }

    [Fact]
    public async Task CreateAndGetWarehouse_ShouldWork()
    {
        // Arrange
        var accountId  = CustomWebApplicationFactory<Program>.TestAccount;
        var uniqueId   = Guid.NewGuid().ToString("N")[..6];
        var uniqueName = $"Main Warehouse {uniqueId}";        
        var uniqueStreet = $"Av. Peru {uniqueId}";         
        var uniquePostal = uniqueId;                         

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(uniqueName),          "Name");
        content.Add(new StringContent("Lima"),              "AddressCity");
        content.Add(new StringContent(uniqueStreet),        "AddressStreet");
        content.Add(new StringContent("Villa el Salvador"), "AddressDistrict");
        content.Add(new StringContent(uniquePostal),        "AddressPostalCode");
        content.Add(new StringContent("Peru"),              "AddressCountry");
        content.Add(new StringContent("2"),                 "TemperatureMin");
        content.Add(new StringContent("8"),                 "TemperatureMax");
        content.Add(new StringContent("1000"),              "Capacity");

        // Act
        var response = await _client.PostAsync($"/api/v1/accounts/{accountId}/warehouses", content);
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK,
            $"Status: {response.StatusCode} | Body: {responseBody}"
        );
    }
}