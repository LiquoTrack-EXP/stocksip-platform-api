using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Domain.Model.Aggregates;

public class WarehouseTest
{
    [Fact]
    public void Constructor_ValidInputs_ShouldCreateWarehouse()
    {
        var name = "Main Warehouse";
        var address = new WarehouseAddress("Av. Principal 123", "Lima", "Miraflores", "15074", "Peru");
        var temperature = new WarehouseTemperature(1, 5);
        var capacity = new WarehouseCapacity(1000);
        var imageUrl = new ImageUrl("https://example.com/warehouse.png");
        var accountId = new AccountId("acc-123");

        var warehouse = new Warehouse(name, address, temperature, capacity, imageUrl, accountId);

        Assert.Equal(name, warehouse.Name);
        Assert.Equal(address, warehouse.Address);
        Assert.Equal(temperature, warehouse.Temperature);
        Assert.Equal(capacity, warehouse.Capacity);
        Assert.Equal(imageUrl, warehouse.ImageUrl);
        Assert.Equal(accountId, warehouse.AccountId);
    }
}
