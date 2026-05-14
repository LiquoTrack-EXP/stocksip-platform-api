using FluentValidation.Validators;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Exceptions;
using NUnit.Framework;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Domain.Model.Aggregates;

[TestFixture]
[TestOf(typeof(Warehouse))]
public class WarehouseTest
{
    // Unit Test
    [Test]
    public void CreateWarehouseShouldSetAllPropertiesCorrectly()
    {
        var command = new RegisterWarehouseCommand(
            "Main Warehouse",
            new WarehouseAddress("Lima", "Av. Peru 123", "Villa el salvador", "12345", "Peru"),
            new WarehouseTemperature(2, 8),
            new WarehouseCapacity(1000),
            null,
            new AccountId("acc_001")
        );

        var imageUrl = "https://res.cloudinary.com/deuy1pr9e/image/upload/v1759709826/Default-warehouse_qdgvkw.jpg";

        var warehouse = new Warehouse(command, imageUrl);

        Assert.That(warehouse.Name, Is.EqualTo("Main Warehouse"));
        Assert.That(warehouse.Address, Is.EqualTo(command.Address));
        Assert.That(warehouse.Capacity, Is.EqualTo(command.Capacity));
        Assert.That(warehouse.Temperature, Is.EqualTo(command.Temperature));
        Assert.That(warehouse.AccountId, Is.EqualTo(command.AccountId));
        Assert.That(warehouse.ImageUrl.GetValue(), Is.EqualTo(imageUrl));
    }

    [Test]
    public void ConstructingWarehouseTemperatureWithInvalidRangeShouldThrowException()
    {
        // Arrange
        var min = 10m;
        var max = 5m;

        // Act & Assert
        Assert.Throws<ValueObjectValidationException>(() => new WarehouseTemperature(min, max));
    }
}