using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Exceptions;
using NUnit.Framework;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Domain.Model.Aggregates;

[TestFixture]
[TestOf(typeof(Product))]
public class ProductTest
{

    // Unit Test
    [Test]
    public void CreateWarehouseShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var command = new RegisterProductCommand(
            "Vino Borgoña",
            EProductTypes.Wines,
            "Borgoña",
            new Money(3.50m, new Currency("PEN")),
            new ProductMinimumStock(10),
            new ProductContent(750),
            null,
            new AccountId("acc_001"),
            null
        );

        var imageUrl = "https://res.cloudinary.com/deuy1pr9e/image/upload/v1759709979/Default-product_kt9bxf.png";

        // Act
        var product = new Product(command, imageUrl);

        // Assert
        Assert.That(product.Name, Is.EqualTo("Vino Borgoña"));
        Assert.That(product.Brand, Is.EqualTo("Borgoña"));
        Assert.That(product.Type, Is.EqualTo(EProductTypes.Wines));
        Assert.That(product.UnitPrice, Is.EqualTo(command.UnitPrice));
        Assert.That(product.MinimumStock, Is.EqualTo(command.MinimumStock));
        Assert.That(product.Content, Is.EqualTo(command.Content));
        Assert.That(product.AccountId, Is.EqualTo(command.AccountId));
        Assert.That(product.SupplierId, Is.EqualTo(command.SupplierId));
        Assert.That(product.TotalStockInStore, Is.EqualTo(0));
        Assert.That(product.IsInWarehouse, Is.False);
        Assert.That(product.ImageUrl.GetValue(), Is.EqualTo(imageUrl));
    }

    [Test]
    public void ConstructingMoneyOrProductMinimumStockWithNegativeValueShouldThrowException()
    {
        // Arrange
        var negativeAmount = -10.5m;
        var currency = new Currency("PEN");
        var negativeMinimumStock = -5;

        // Act & Assert
        Assert.Throws<ValueObjectValidationException>(() => new Money(negativeAmount, currency));
        Assert.Throws<ValueObjectValidationException>(() => new ProductMinimumStock(negativeMinimumStock));
    }
    
    // Integration Test
}