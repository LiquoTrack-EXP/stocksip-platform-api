using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.OutboundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using NUnit.Framework;
using Moq;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Application.Internal.CommandServices;

[TestFixture]
[TestOf(typeof(WarehouseCommandService))]
public class WarehouseCommandServiceTest
{
    private Mock<IWarehouseRepository> _warehouseRepositoryMock;
    private Mock<IInventoryImageService> _imageServiceMock;
    private Mock<IPaymentAndSubscriptionsFacade> _paymentFacadeMock;

    private WarehouseCommandService _service;

    [SetUp]
    public void Setup()
    {
        _warehouseRepositoryMock = new Mock<IWarehouseRepository>();
        _imageServiceMock = new Mock<IInventoryImageService>();
        _paymentFacadeMock = new Mock<IPaymentAndSubscriptionsFacade>();

        _service = new WarehouseCommandService(
            _warehouseRepositoryMock.Object,
            _imageServiceMock.Object,
            _paymentFacadeMock.Object
        );
    }

    // Integration Test
    [Test]
    public async Task HandleShouldCreateWarehouseWhenValidCommand()
    {
        // Arrange
        var command = new RegisterWarehouseCommand(
            "Main Warehouse",
            new WarehouseAddress("Lima", "Av. Peru 123", "Lima", "15001", "Peru"),
            new WarehouseTemperature(2, 8),
            new WarehouseCapacity(1000),
            null,
            new AccountId("acc_001")
        );

        _paymentFacadeMock
            .Setup(x => x.GetPlanWarehouseLimitByAccountId("acc_001"))
            .ReturnsAsync(5);

        _warehouseRepositoryMock
            .Setup(x => x.CountByAccountIdAsync(command.AccountId))
            .ReturnsAsync(1);

        _warehouseRepositoryMock
            .Setup(x => x.ExistByNameIgnoreCaseAndAccountIdAsync(command.Name, command.AccountId))
            .ReturnsAsync(false);

        _warehouseRepositoryMock
            .Setup(x => x.ExistsByStreetAndCityAndPostalCodeIgnoreCaseAndAccountIdAsync(
                command.Address.Street,
                command.Address.City,
                command.Address.PostalCode,
                command.AccountId))
            .ReturnsAsync(false);

        _warehouseRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Warehouse>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Handle(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Main Warehouse"));
    }
}