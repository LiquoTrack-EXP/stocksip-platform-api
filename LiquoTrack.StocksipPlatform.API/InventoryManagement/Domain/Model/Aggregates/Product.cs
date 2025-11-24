using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.Entities;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;

/// <summary>
///     The Product Aggregate Root entity.
/// </summary>
public class Product(
    string name,
    EProductTypes type,
    string brand,
    Money unitPrice,
    ProductMinimumStock minimumStock,
    ProductContent content,
    ImageUrl imageUrl,
    AccountId accountId,
    AccountId supplierId
) : Entity
{
    /// <summary>
    ///     The name of the product.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    ///     The type of the product.
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    public EProductTypes Type { get; private set; } = type;

    /// <summary>
    ///     The brand of the product.
    /// </summary>
    public string Brand { get; private set; } = brand;
    
    /// <summary>
    ///     The unit price of the product.
    /// </summary>
    public Money UnitPrice { get; private set; } = unitPrice;

    /// <summary>
    ///     The minimum stock of the product before it is considered low of stock.
    /// </summary>
    public ProductMinimumStock MinimumStock { get; private set; } = minimumStock;

    /// <summary>
    ///     The content of the product.
    /// </summary>
    public ProductContent Content { get; private set; } = content;

    /// <summary>
    ///     The total stock in the store. Which sums up the stock of the product in all the warehouses.
    /// </summary>
    public int TotalStockInStore { get; private set; } = 0;
    
    /// <summary>
    ///     Indicates if the product is in the warehouse.
    /// </summary>
    public bool IsInWarehouse { get; private set; } = false;
    
    /// <summary>
    ///     The image url of the product.
    /// </summary>
    public ImageUrl ImageUrl { get; private set; } = imageUrl;

    /// <summary>
    ///     The identifier of the account that owns the product.
    /// </summary>
    public AccountId AccountId { get; private set; } = accountId;

    /// <summary>
    ///     The identifier of the supplier of the product. Can be null.
    /// </summary>
    public AccountId? SupplierId { get; private set; } = supplierId;

    /// <summary>
    ///     Command constructor for the Product Aggregate Root.
    /// </summary>
    /// <param name="command">
    ///     The command that triggered the creation of the product.
    ///     It contains the information needed to create the product.   
    /// </param>
    public Product(RegisterProductCommand command, string imageUrl) : this(command.Name, command.Type, command.Brand, command.UnitPrice,
        command.MinimumStock, command.Content, new ImageUrl(imageUrl), command.AccountId, command.SupplierId)
    { }
    
    /// <summary>
    ///     Method to update the minimum stock of the product.
    /// </summary>
    /// <param name="command">
    ///     The command containing the details for updating the minimum stock of the product.
    /// </param>
    public void UpdateMinimumStock(UpdateProductMinimumStockCommand command) => MinimumStock.UpdateMinimumStock(command.NewMinimumStock);

    /// <summary>
    ///     Method to update the total stock in store of the product.
    /// </summary>
    /// <param name="totalStockInStore">
    ///     The new total stock in store value.
    /// </param>
    /// <exception cref="ArgumentException">
    ///     Thrown when the total stock in the store is negative.
    /// </exception>
    public void UpdateTotalStockInStore(int totalStockInStore)
    {
        if (totalStockInStore < 0)
            throw new ArgumentException("Total stock in store cannot be negative.", nameof(totalStockInStore));

        if (totalStockInStore > 0) StoreProduct();
        if (totalStockInStore == 0) IsInWarehouse = false;
        
        TotalStockInStore = totalStockInStore;
    }
    
    /// <summary>
    ///     Method to mark the product as being in the warehouse.
    /// </summary>
    public void StoreProduct() => IsInWarehouse = true;

    /// <summary>
    ///     Method to update the product information.
    /// </summary>
    /// <param name="command">
    ///     The command containing the new product information. 
    /// </param>
    public void UpdateInformation(UpdateProductInformationCommand command, string imageUrl) 
    {
        Name = command.Name;
        UnitPrice = command.UnitPrice;
        MinimumStock = command.MinimumStock;
        ImageUrl = new ImageUrl(imageUrl);
    }
    
    /// <summary>
    ///     Getter method for the total stock in store.
    /// </summary>
    /// <returns>
    ///     The total stock in store of the product.
    /// </returns>
    public int GetStockInStorage() => TotalStockInStore;
}