using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.CommandServices;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Application.Internal.OutboundServices.FileStorage;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.PaymentAndSubscriptions.Interfaces.ACL.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Application.Internal.CommandServices;

[TestFixture]
[TestOf(typeof(ProductCommandService))]
public class ProductCommandServiceTest
{
    private Mock<IProductRepository> _productRepositoryMock;
    private Mock<IInventoryImageService> _imageServiceMock;
    private Mock<IPaymentAndSubscriptionsFacade> _paymentFacadeMock;
    
    private ProductCommandService _service;
    
    [SetUp]
    public void Setup()    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _imageServiceMock = new Mock<IInventoryImageService>();
        _paymentFacadeMock = new Mock<IPaymentAndSubscriptionsFacade>();

        _service = new ProductCommandService(
            _productRepositoryMock.Object,
            _imageServiceMock.Object,
            null,
            _paymentFacadeMock.Object
        );
    }
    

    [Test]
    public async Task Handle_ShouldCreateProduct_WhenValidCommand()
    {
        // Arrange
        var command = new RegisterProductCommand(
            "Vino Borgoña",
            EProductTypes.Wines,
            "Borgoña",
            new Money(10, new Currency("PEN")),
            new ProductMinimumStock(5),
            new ProductContent(750),
            null,
            new AccountId("acc_001"),
            null
        );

        _productRepositoryMock
            .Setup(x => x.ExistsByNameAndAccountIdAsync(It.IsAny<ProductName>(), command.AccountId))
            .ReturnsAsync(false);

        _productRepositoryMock
            .Setup(x => x.CountByAccountIdAsync(command.AccountId))
            .ReturnsAsync(1);

        _paymentFacadeMock
            .Setup(x => x.GetPlanWarehouseLimitByAccountId("acc_001"))
            .ReturnsAsync(10);
        

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.Handle(command);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Vino Borgoña"));
    }
}