using System.Net.Mime;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Commands;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Queries;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Services;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Assemblers;
using LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace LiquoTrack.StocksipPlatform.API.InventoryManagement.Interfaces.REST.Controllers;

/// <summary>
///     Controller class for handling warehouse-related requests.
/// </summary>
/// <param name="inventoryCommandService">
///     The service for handling warehouse-related commands.
/// </param>
/// <param name="inventoryQueryService">
///     The service for handling warehouse-related queries.
/// </param>
/// <param name="productQueryService">
///     The service for handling product-related queries.
/// </param>
[ApiController]
[Route("api/v1/warehouses/{warehouseId}/products")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Warehouses")]
public class WarehouseProductsController(
        IInventoryCommandService inventoryCommandService,
        IInventoryQueryService inventoryQueryService,
        IProductQueryService productQueryService,
        IProductExitQueryService productExitQueryService
    ) : ControllerBase
{
    /// <summary>
    ///     Method to try parsing the route parameters into valid object IDs.
    /// </summary>
    /// <returns>
    ///     A boolean value indicating whether the route parameters could be parsed successfully.
    /// </returns>
    private bool TryParseObjectIds(string warehouseId, string productId, out ObjectId warehouseObjId, out ObjectId productObjId, out IActionResult? error)
    {
        error = null;
        warehouseObjId = default;
        productObjId = default;

        if (!ObjectId.TryParse(warehouseId, out warehouseObjId))
        {
            error = BadRequest("Invalid warehouse ID.");
            return false;
        }

        if (ObjectId.TryParse(productId, out productObjId)) return true;
        error = BadRequest("Invalid product ID.");
        
        return false;

    }
    
    /// <summary>
    ///     Endpoint to handle the retrieval of an inventory by its ID.
    /// </summary>
    /// <param name="warehouseId"></param>
    /// <param name="productId"></param>
    /// <param name="expirationDate"></param>
    /// <returns></returns>
    [HttpGet("{productId}")]
    [SwaggerOperation(
        Summary = "Get Inventory by Product ID and Warehouse ID",
        Description = "Retrieves inventory details for a specific product in a specific warehouse. Optionally, filter by expiration date.",
        OperationId = "GetInventoryByProductIdAndWarehouseId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Inventory found!", typeof(InventoryWithProductResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Inventory not found.")]
    public async Task<IActionResult> GetInventoryByProductIdAndWarehouseId(
        [FromRoute] string warehouseId, 
        [FromRoute] string productId,
        [FromQuery] DateTime? expirationDate)
    {
        if (!ObjectId.TryParse(warehouseId, out var warehouseObjId)) return BadRequest("Invalid warehouse ID.");
        if (!ObjectId.TryParse(productId, out var productObjId)) return BadRequest("Invalid product ID.");

        Inventory? inventory;
        if (expirationDate.HasValue)
        {
            var expiration = new ProductExpirationDate(DateOnly.FromDateTime(expirationDate.Value));
            var q = new GetInventoryByProductIdWarehouseIdAndExpirationDateQuery(productObjId, warehouseObjId, expiration);
            inventory = await inventoryQueryService.Handle(q);
        }
        else
        {
            var q = new GetInventoryByProductIdAndWarehouseIdQuery(productObjId, warehouseObjId);
            inventory = await inventoryQueryService.Handle(q);
        }

        if (inventory is null) return NotFound("Inventory not found.");
        var product = await productQueryService.Handle(new GetProductByIdQuery(productObjId));
        if (product is null) return NotFound("Product not found.");
        var resource = InventoryWithProductResourceFromEntityAssembler.ToResourceFromEntity(inventory, product);
        return Ok(resource);
    }

    /// <summary>
    ///     Endpoint to handle the retrieval of a product exit by its ID.
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse for which to retrieve the product exit.
    /// </param>
    /// <param name="productId">
    ///     The route parameter representing the unique identifier of the product for which to retrieve the product exit.
    /// </param>
    /// <param name="expirationDate">
    ///     The query parameter representing the expiration date of the product exit to be retrieved.
    /// </param>
    /// <returns></returns>
    [HttpGet("{productId}/exits")]
    [SwaggerOperation(
        Summary = "Get Product Exit by Product ID and Warehouse ID",
        Description = "Retrieves product exit details for a specific product in a specific warehouse. Optionally, filter by expiration date.",
        OperationId = "GetProductExitByProductIdAndWarehouseId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Product exit found!", typeof(ProductExitResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product exit not found.")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid warehouse ID.")]
    public async Task<IActionResult> GetProductExitByWarehouseIdAndProductId(
        [FromRoute] string warehouseId,
        [FromRoute] string productId,
        [FromQuery] DateTime? expirationDate)
    {
        if (!ObjectId.TryParse(warehouseId, out var warehouseObjId)) return BadRequest("Invalid warehouse ID.");
        if (!ObjectId.TryParse(productId, out var productObjId)) return BadRequest("Invalid product ID.");
        
        var getProductExitByWarehouseIdQuery = new GetProductExitByProductIdAndWarehouseIdQuery(warehouseObjId, productObjId, expirationDate);
        var productExit = await productExitQueryService.Handle(getProductExitByWarehouseIdQuery);
        if (productExit is null) return NotFound("Product exit not found.");
        var resource = ProductExitResourceFromEntityAssembler.ToResourceFromEntity(productExit);
        return Ok(resource);
    }
    
    /// <summary>
    ///     Endpoint to handle the retrieval of all products in a warehouse.   
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse for which to retrieve products. 
    /// </param>
    /// <returns>
    ///     A list of products in the specified warehouse, or a 404 Not Found response if no products are found for the given warehouse ID.
    /// </returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get all products in a warehouse",
        Description = "Retrieves all products in a specific warehouse.",
        OperationId = "GetAllProductsByWarehouseId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Products found!", typeof(List<InventoryWithProductResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No products found.")]
    public async Task<IActionResult> GetAllProductsByWarehouseId([FromRoute] string warehouseId)
    {
        if (!ObjectId.TryParse(warehouseId, out var warehouseObjId)) return BadRequest("Invalid warehouse ID.");

        var getAllInventoriesByWarehouseIdQuery = new GetAllInventoriesByWarehouseIdQuery(warehouseObjId);
        var inventories = await inventoryQueryService.Handle(getAllInventoriesByWarehouseIdQuery) ?? Enumerable.Empty<Inventory>();
        var enumerable = inventories as Inventory[] ?? inventories.ToArray();

        var productIds = enumerable.Select(i => i.ProductId).Distinct().ToList();
        var products = new List<Product>();
        foreach (var productId in productIds)
        {
            var product = await productQueryService.Handle(new GetProductByIdQuery(productId));
            if (product != null) products.Add(product);
        }

        var productDict = products.ToDictionary(p => p.Id, p => p);
        var resources = enumerable
            .Where(inv => productDict.ContainsKey(inv.ProductId))
            .Select(inv => InventoryWithProductResourceFromEntityAssembler.ToResourceFromEntity(inv, productDict[inv.ProductId]))
            .ToList();

        return Ok(resources);
    }

    /// <summary>
    ///     Endpoint to handle the addition of products to a warehouse.  
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse to which products are to be added.
    /// </param>
    /// <param name="productId">
    ///     The route parameter representing the unique identifier of the product to be added.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the details of the products to be added.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the added products, or a 400 Bad Request response if the products could not be added. 
    /// </returns>
    [HttpPost("{productId}/additions")]
    [SwaggerOperation(
        Summary = "Add products to warehouse",
        Description = "Adds products to a specific warehouse.",
        OperationId = "AddProductsToWarehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Products added successfully!", typeof(InventoryWithProductResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Products could not be added...")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found.")]
    public async Task<IActionResult> AddProductsToWarehouse(
        [FromRoute] string warehouseId, 
        [FromRoute] string productId,
        [FromBody] AddProductsToWarehouseResource resource)
    {
        if (!ObjectId.TryParse(warehouseId, out var warehouseObjId)) return BadRequest("Invalid warehouse ID.");
        if (!ObjectId.TryParse(productId, out var productObjId)) return BadRequest("Invalid product ID.");

        Inventory? inventory;
        if (resource.ExpirationDate.HasValue)
        {
            var expiration = new ProductExpirationDate(DateOnly.FromDateTime(resource.ExpirationDate.Value));
            var cmd = new AddProductsToWarehouseCommand(productObjId, warehouseObjId, expiration, resource.QuantityToAdd);
            inventory = await inventoryCommandService.Handle(cmd);
        }
        else
        {
            var cmd = new AddProductsToWarehouseWithoutExpirationDateCommand(productObjId, warehouseObjId, resource.QuantityToAdd);
            inventory = await inventoryCommandService.Handle(cmd);
        }

        if (inventory is null) return BadRequest("Products could not be added.");
        var product = await productQueryService.Handle(new GetProductByIdQuery(productObjId));
        if (product is null) return NotFound("Product not found.");
        var resourceWithProduct = InventoryWithProductResourceFromEntityAssembler.ToResourceFromEntity(inventory, product);
        return Ok(resourceWithProduct);
    }

    /// <summary>
    ///     Endpoint to handle the subtraction of products from a warehouse. 
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse from which products are to be subtracted.
    /// </param>
    /// <param name="productId">
    ///     The route parameter representing the unique identifier of the product to be subtracted.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the details of the products to be subtracted.
    /// </param>
    /// <returns>
    ///     A 200 OK response with the details of the subtracted products, or a 400 Bad Request response if the products could not be subtracted.
    /// </returns>
    [HttpPost("{productId}/subtractions")]
    [SwaggerOperation(
        Summary = "Subtract products from warehouse",
        Description = "Subtracts products from a specific warehouse.",
        OperationId = "SubtractProductsFromWarehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Products subtracted successfully!", typeof(InventoryWithProductResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Products could not be subtracted...")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found.")]
    public async Task<IActionResult> SubtrackProductsFromWarehouse(
        [FromRoute] string warehouseId,
        [FromRoute] string productId,
        [FromBody] DecreaseProductsFromWarehouseResource resource)
    {
        if (!ObjectId.TryParse(warehouseId, out var warehouseObjId)) return BadRequest("Invalid warehouse ID.");
        if (!ObjectId.TryParse(productId, out var productObjId)) return BadRequest("Invalid product ID.");

        var exitType = Enum.Parse<EProductExitReasons>(resource.ExitType);
        
        Inventory? inventory;
        if (resource.ExpirationDate.HasValue)
        {
            var expiration = new ProductExpirationDate(DateOnly.FromDateTime(resource.ExpirationDate.Value));
            var cmd = new DecreaseProductsFromWarehouseCommand(productObjId, warehouseObjId, expiration, resource.QuantityToDecrease, exitType);
            inventory = await inventoryCommandService.Handle(cmd);
        }
        else
        {
            var cmd = new DecreaseProductsFromWarehouseWithoutExpirationDateCommand(productObjId, warehouseObjId, resource.QuantityToDecrease, exitType);
            inventory = await inventoryCommandService.Handle(cmd);
        }

        if (inventory is null) return BadRequest("Products could not be subtracted from the warehouse.");
        var product = await productQueryService.Handle(new GetProductByIdQuery(productObjId));
        if (product is null) return NotFound("Product not found.");
        var resourceWithProduct = InventoryWithProductResourceFromEntityAssembler.ToResourceFromEntity(inventory, product);
        return Ok(resourceWithProduct);
    }

    /// <summary>
    ///     Endpoint to handle the transfer of products to another warehouse.
    /// </summary>
    /// <param name="warehouseId">
    ///     The route parameter representing the unique identifier of the warehouse to which products are to be transferred.
    /// </param>
    /// <param name="productId">
    ///     The route parameter representing the unique identifier of the product to be transferred.
    /// </param>
    /// <param name="resource">
    ///     The request body containing the details of the products to be transferred.  
    /// </param>
    /// <returns>
    ///     The updated current inventory for the product, or a 400 Bad Request response if the products could not be transferred.
    /// </returns>
    [HttpPost("{productId}/transfers")]
    [SwaggerOperation(
        Summary = "Transfer products to another warehouse",
        Description = "Transfers products from one warehouse to another.",
        OperationId = "TransferProductsToAnotherWarehouse")]
    [SwaggerResponse(StatusCodes.Status200OK, "Products transferred successfully!", typeof(InventoryWithProductResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Products could not be transferred...")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Product not found.")]
    public async Task<IActionResult> TransferProductsToAnotherWarehouse(
            [FromRoute] string warehouseId,
            [FromRoute] string productId, 
            [FromBody] TransferProductsToAnotherWarehouseResource resource
        )
    {
        if (!ObjectId.TryParse(warehouseId, out var originWarehouseObjId)) return BadRequest("Invalid origin warehouse ID.");
        if (!ObjectId.TryParse(productId, out var productObjId)) return BadRequest("Invalid product ID.");
        if (!ObjectId.TryParse(resource.DestinationWarehouseId, out var destinationWarehouseObjId)) return BadRequest("Invalid destination warehouse ID.");

        var product = await productQueryService.Handle(new GetProductByIdQuery(productObjId));
        if (product is null) return NotFound("Product not found.");

        var command = TransferProductsToAnotherWarehouseCommandFromResourceAssembler.ToCommandFromResource(resource, originWarehouseObjId.ToString(), productObjId.ToString());
        var inventory = await inventoryCommandService.Handle(command);
        if (inventory is null) return BadRequest("Products could not be transferred.");
        var resourceWithProduct = InventoryWithProductResourceFromEntityAssembler.ToResourceFromEntity(inventory, product);
        return Ok(resourceWithProduct);
    }
}