using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.OpenApi.Extensions;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Domain.Model.Aggregates;

/// <summary>
///     Class for testing the Product Aggregate Root entity.
/// </summary>
public class ProductTests
{
    /// <summary>
    ///     Method to test the constructor of the Product Aggregate Root.
    /// </summary>
    [Fact]
    public void Constructor_ValidInputs_ShouldCreateProducto()
    {
        // Arrange
        const string name = "Whisky Blue Label";
        const decimal price = 10.99m;
        var brand = EBrandNames.JhonnyWalker.GetDisplayName();
        const int minimumStock = 5;
        const decimal content = 300.00m;
        const string imageUrl = "https://www.example.com/image.jpg";
        const string ownerId = "1234567890";
        const string supplierId = "1234555";

        // Act
        var unitPrice = new Money(price, new Currency(EValidCurrencyCodes.PEN.GetDisplayName()));
        var productMinimumStock = new ProductMinimumStock(minimumStock);
        var productContent = new ProductContent(content);
        var imageUrlObject = new ImageUrl(imageUrl);
        var accountId = new AccountId(ownerId);
        var supplierIdObject = new AccountId(supplierId);
        var product = new Product(name, EProductTypes.Whiskeys, brand, unitPrice, productMinimumStock, productContent, imageUrlObject, accountId, supplierIdObject);

        // Assert
        Assert.Equal(name, product.Name);
        Assert.Equal(EProductTypes.Whiskeys, product.Type);
        Assert.Equal(brand, product.Brand);
        Assert.Equal(unitPrice, product.UnitPrice);
        Assert.Equal(productMinimumStock, product.MinimumStock);
        Assert.Equal(productContent, product.Content);
        Assert.Equal(imageUrlObject, product.ImageUrl);
        Assert.Equal(accountId, product.AccountId);
        Assert.Equal(supplierIdObject, product.SupplierId);
        Assert.Equal(0, product.TotalStockInStore);
    }

    /// <summary>
    ///     Method to test the constructor of the Product Aggregate Root.
    /// </summary>
    /// <param name="types">
    ///     The product type to test.
    /// </param>
    [Theory]
    [InlineData(EProductTypes.Beers)]
    [InlineData(EProductTypes.Cocktails)]
    [InlineData(EProductTypes.Rums)]
    [InlineData(EProductTypes.Tequilas)]
    [InlineData(EProductTypes.Vodkas)]
    public void Constructor_ShouldAcceptAllValidProductTypesEnumValues(EProductTypes types)
    {
        // Arrange
        const string name = "Whisky Blue Label";
        const decimal price = 10.99m;
        var brand = EBrandNames.JhonnyWalker.GetDisplayName();
        const int minimumStock = 5;
        const decimal content = 300.00m;
        const string imageUrl = "https://www.example.com/image.jpg";
        const string ownerId = "1234567890";
        const string supplierId = "1234555";
        
        // Act
        var unitPrice = new Money(price, new Currency(EValidCurrencyCodes.PEN.GetDisplayName()));
        var productMinimumStock = new ProductMinimumStock(minimumStock);
        var productContent = new ProductContent(content);
        var imageUrlObject = new ImageUrl(imageUrl);
        var accountId = new AccountId(ownerId);
        var supplierIdObject = new AccountId(supplierId);
        var product = new Product(name, types, brand, unitPrice, productMinimumStock, productContent, imageUrlObject, accountId, supplierIdObject);


        Assert.Equal(types, product.Type);
    }

    /// <summary>
    ///     Method to test the inventory behavior of the Product Aggregate Root.
    /// </summary>
    [Fact]
    public void InventoryTest_ShouldUpdateTotalStockAndWarehouseFlag()
    {
        // Arrange
        const string name = "Whisky Blue Label";
        const decimal price = 10.99m;
        var brand = EBrandNames.JhonnyWalker.GetDisplayName();
        const int minimumStock = 5;
        const decimal content = 300.00m;
        const string imageUrl = "https://www.example.com/image.jpg";
        const string ownerId = "1234567890";
        const string supplierId = "1234555";
        const int expectedStock = 15;

        var unitPrice = new Money(price, new Currency(EValidCurrencyCodes.PEN.GetDisplayName()));
        var productMinimumStock = new ProductMinimumStock(minimumStock);
        var productContent = new ProductContent(content);
        var imageUrlObject = new ImageUrl(imageUrl);
        var accountId = new AccountId(ownerId);
        var supplierIdObject = new AccountId(supplierId);
        var product = new Product(name, EProductTypes.Whiskeys, brand, unitPrice, productMinimumStock, productContent, imageUrlObject, accountId, supplierIdObject);

        // Act
        product.UpdateTotalStockInStore(expectedStock);
        product.StoreProduct();

        // Assert
        Assert.Equal(expectedStock, product.TotalStockInStore);
        Assert.True(product.IsInWarehouse);
    }
}