using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;

[TestFixture]
public class InventoryCommandServiceTest
{
    private Mock<IProductRepository> productRepositoryMock;
    private Mock<IWarehouseRepository> warehouseRepositoryMock;
    private Mock<IInventoryRepository> inventoryRepositoryMock;
    private Mock<IProductExitRepository> productExitRepositoryMock;
    private Mock<IProductTransferRepository> productTransferRepositoryMock;

    private InventoryCommandService service;

    [SetUp]
    public void Setup()
    {
        productRepositoryMock = new Mock<IProductRepository>();
        warehouseRepositoryMock = new Mock<IWarehouseRepository>();
        inventoryRepositoryMock = new Mock<IInventoryRepository>();
        productExitRepositoryMock = new Mock<IProductExitRepository>();
        productTransferRepositoryMock = new Mock<IProductTransferRepository>();

        service = new InventoryCommandService(
            productRepositoryMock.Object,
            warehouseRepositoryMock.Object,
            inventoryRepositoryMock.Object,
            productExitRepositoryMock.Object,
            productTransferRepositoryMock.Object
        );
    }
    
    [Test]
    public async Task HandleShouldAssignProductToWarehouseWhenValidData()
    {
        // Arrange
        var productId = ObjectId.GenerateNewId();
        var warehouseId = ObjectId.GenerateNewId();

        var product = new Product(
            "Vino Borgoña",
            EProductTypes.Wines,
            "Borgoña",
            new Money(8, new Currency("PEN")),
            new ProductMinimumStock(3),
            new ProductContent(750),
            null,
            new AccountId("acc_001")
        );

        var warehouse = new Warehouse(
            "Central Warehouse",
            new WarehouseAddress(
                "Lima",
                "Av Peru 123",
                "Surco",
                "15001",
                "Peru"
            ),
            new WarehouseTemperature(2, 8),
            new WarehouseCapacity(500),
            null,
            new AccountId("acc_001")
        );

        var command = new AddProductsToWarehouseWithoutExpirationDateCommand(
            productId,
            warehouseId,
            20
        );
        
        productRepositoryMock
            .Setup(x => x.FindByIdAsync(productId.ToString()))
            .ReturnsAsync(product);
        
        warehouseRepositoryMock
            .Setup(x => x.FindByIdAsync(warehouseId.ToString()))
            .ReturnsAsync(warehouse);
        
        inventoryRepositoryMock
            .Setup(x => x.GetByProductIdWarehouseIdAsync(
                productId,
                warehouseId))
            .ReturnsAsync((Inventory?)null);

        inventoryRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Inventory>()))
            .Returns(Task.CompletedTask);

        productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await service.Handle(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        
        Assert.That(result!.GetStock(), Is.EqualTo(20));
        
        inventoryRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Inventory>()),
            Times.Once
        );
        
        productRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Product>()),
            Times.Once
        );
    }

    [Test]
    public async Task HandleAddStockShouldIncreaseStockWhenValidData()
    {
        // Arrange
        var productObjectId = ObjectId.GenerateNewId();
        var warehouseObjectId = ObjectId.GenerateNewId();

        var product = new Product(
            "Vino Borgoña",
            EProductTypes.Wines,
            "Borgoña",
            new Money(10, new Currency("PEN")),
            new ProductMinimumStock(2),
            new ProductContent(100),
            null,
            new AccountId("account_01")
        );

        var warehouse = new Warehouse(
            "Main Warehouse",
            new WarehouseAddress("Lima", "Av. Peru 123", "Villa el salvador", "12345", "Peru"),
            new WarehouseTemperature(2, 8),
            new WarehouseCapacity(1000),
            null,
            new AccountId("acc_001")
        );

        var command = new AddProductsToWarehouseWithoutExpirationDateCommand(
            productObjectId,
            warehouseObjectId,
            5
        );
        productRepositoryMock
            .Setup(x => x.FindByIdAsync(productObjectId.ToString()))
            .ReturnsAsync(product);

        warehouseRepositoryMock
            .Setup(x => x.FindByIdAsync(warehouseObjectId.ToString()))
            .ReturnsAsync(warehouse);

        inventoryRepositoryMock
            .Setup(x => x.GetByProductIdWarehouseIdAsync(
                productObjectId,
                warehouseObjectId))
            .ReturnsAsync((Inventory)null);

        // Act
        var result = await service.Handle(command);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.GetStock(), Is.EqualTo(5));
    }

}