using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.ACL;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Repositories;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Services;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Application.Internal.CommandServices;

/// <summary>
/// Command service responsible for handling catalog-related operations.
/// </summary>
public class CatalogCommandService : ICatalogCommandService
{
    private readonly ICatalogRepository _catalogRepository;
    private readonly IProductContextFacade _productContextFacade;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public CatalogCommandService(
        ICatalogRepository catalogRepository,
        IProductContextFacade productContextFacade,
        IWarehouseRepository warehouseRepository,
        IInventoryRepository inventoryRepository)
    {
        _catalogRepository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
        _productContextFacade = productContextFacade ?? throw new ArgumentNullException(nameof(productContextFacade));
        _warehouseRepository = warehouseRepository ?? throw new ArgumentNullException(nameof(warehouseRepository));
        _inventoryRepository = inventoryRepository ?? throw new ArgumentNullException(nameof(inventoryRepository));
    }

    public async Task<CatalogId> Handle(CreateCatalogCommand command)
    {
        var catalog = new Catalog(command);
        await _catalogRepository.AddAsync(catalog);
        return catalog.CatalogId;
    }

    public async Task Handle(UpdateCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.UpdateCatalog(command);
        await _catalogRepository.UpdateAsync(catalog);
    }

    public async Task Handle(PublishCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.PublishCatalog();
        await _catalogRepository.UpdateAsync(catalog);
    }

    public async Task Handle(UnpublishCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.UnpublishCatalog();
        await _catalogRepository.UpdateAsync(catalog);
    }

    /// <summary>
    /// Handles the AddItemToCatalogCommand.
    /// Fetches product details and stock from the Inventory context and adds it to the catalog.
    /// Additionally, it reduces the assigned stock from the physical inventory.
    /// </summary>
    public async Task Handle(AddItemToCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.CatalogId);
        
        var product = await _productContextFacade.GetProductDetailsByIdAsync(command.ProductId)
                      ?? throw new InvalidOperationException($"Product with ID {command.ProductId} not found");

        int stockToAssign = command.Stock > 0 ? command.Stock
            : (await _productContextFacade.GetProductStockInWarehouseAsync(command.ProductId, command.WarehouseId)) 
              ?? throw new InvalidOperationException($"No inventory found for product {command.ProductId} in warehouse {command.WarehouseId}");

        var productObjectId = new ObjectId(command.ProductId);
        var warehouseObjectId = new ObjectId(command.WarehouseId);
        
        var inventoryItem = await _inventoryRepository.GetByProductIdWarehouseIdAsync(productObjectId, warehouseObjectId)
                            ?? throw new InvalidOperationException($"No inventory found for product {command.ProductId} in warehouse {command.WarehouseId}");

        if (inventoryItem.Quantity.GetValue < stockToAssign)
            throw new InvalidOperationException($"Insufficient stock in inventory for product {command.ProductId}. Available: {inventoryItem.Quantity.GetValue}, Requested: {stockToAssign}");
        
        var accountIdString = await _warehouseRepository.FindAccountIdByWarehouseIdAsync(command.WarehouseId);
        var accountId = new AccountId(accountIdString);

        var minimumStock = product.MinimumStock;
        
        inventoryItem.DecreaseStockFromProduct(stockToAssign, minimumStock, accountId);
        await _inventoryRepository.UpdateAsync(inventoryItem);
        
        catalog.AddItem(product.Id, product.Name, product.UnitPrice, product.Currency, product.ImageUrl, stockToAssign);
        await _catalogRepository.UpdateAsync(catalog);
    }

    public async Task Handle(RemoveItemFromCatalogCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.catalogId);
        catalog.RemoveItem(command);
        await _catalogRepository.UpdateAsync(catalog);
    }

    private async Task<Catalog> GetCatalogByIdAsync(string catalogId)
    {
        var id = new CatalogId(catalogId);
        return await _catalogRepository.GetByIdAsync(id)
            ?? throw new InvalidOperationException($"Catalog with ID {catalogId} not found");
    }

    /// <summary>
    /// Handles the ReduceCatalogItemStockCommand.
    /// Decreases the stock of a product within the specified catalog.
    /// </summary>
    public async Task Handle(ReduceCatalogItemStockCommand command)
    {
        var catalog = await GetCatalogByIdAsync(command.CatalogId);

        var item = catalog.CatalogItems.FirstOrDefault(i => i.ProductId == command.ProductId);
        if (item == null)
            throw new InvalidOperationException($"Product with ID {command.ProductId} not found in catalog {command.CatalogId}");

        if (!item.HasSufficientStock(command.Quantity))
            throw new InvalidOperationException($"Insufficient stock for product {command.ProductId}. Available: {item.AvailableStock ?? 0}, Requested: {command.Quantity}");

        var newStock = (item.AvailableStock ?? 0) - command.Quantity;
        item.UpdateStock(newStock);

        await _catalogRepository.UpdateAsync(catalog);
    }
}
