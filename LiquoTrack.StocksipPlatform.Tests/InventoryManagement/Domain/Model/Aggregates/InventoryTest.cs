using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using NUnit.Framework;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Domain.Model.Aggregates;

[TestFixture]
[TestOf(typeof(Inventory))]
public class InventoryTest
{

    // Unit Test
    [Test]
    public void AssignProductToWarehouseShouldSetAllPropertiesCorrectly()
    {
        var productId = new ObjectId();
        var warehouseId = new ObjectId();

        var quantity = new ProductStock(10);

        var expirationDate = new ProductExpirationDate(DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(6)));

        // Act
        var inventory = new Inventory(
            productId,
            warehouseId,
            quantity,
            expirationDate
        );

        // Assert
        Assert.That(inventory.ProductId, Is.EqualTo(productId));
        Assert.That(inventory.WarehouseId, Is.EqualTo(warehouseId));
        Assert.That(inventory.Quantity.GetValue, Is.EqualTo(10));
        Assert.That(inventory.CurrentState, Is.EqualTo(EProductStates.WithStock));
        Assert.That(inventory.ExpirationDate, Is.EqualTo(expirationDate));
    }

    [Test]
    public void AddStockToProductShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var inventory = new Inventory(
            productId: ObjectId.GenerateNewId(),
            warehouseId: ObjectId.GenerateNewId(),
            quantity: new ProductStock(10),
            expirationDate: null
        );

        // Act
        inventory.AddStockToProduct(
            addedStock: 5,
            productMinimumStock: 8
        );

        // Assert
        Assert.That(inventory.GetStock(), Is.EqualTo(15));
    }

    [Test]
    public void DecreaseStockFromProductShouldSetStateToLowStockWhenBelowMinimum()
    {
        // Arrange
        var inventory = new Inventory(
            productId: ObjectId.GenerateNewId(),
            warehouseId: ObjectId.GenerateNewId(),
            quantity: new ProductStock(10),
            expirationDate: null
        );
        var minimumStock = 7;
        var removedStock = 4; 
        var accountId = new AccountId("acc_001");

        // Act
        inventory.DecreaseStockFromProduct(removedStock, minimumStock, accountId);

        // Assert
        Assert.That(inventory.GetStock(), Is.EqualTo(6));
        Assert.That(inventory.CurrentState, Is.EqualTo(EProductStates.LowStock));
    }

    [Test]
    public void DecreaseStockFromProductShouldSetStateToOutOfStockWhenStockIsZero()
    {
        // Arrange
        var inventory = new Inventory(
            productId: ObjectId.GenerateNewId(),
            warehouseId: ObjectId.GenerateNewId(),
            quantity: new ProductStock(3),
            expirationDate: null
        );
        var minimumStock = 1;
        var removedStock = 3;
        var accountId = new AccountId("acc_001");

        // Act
        inventory.DecreaseStockFromProduct(removedStock, minimumStock, accountId);

        // Assert
        Assert.That(inventory.GetStock(), Is.EqualTo(0));
        Assert.That(inventory.CurrentState, Is.EqualTo(EProductStates.OutOfStock));
    }
}