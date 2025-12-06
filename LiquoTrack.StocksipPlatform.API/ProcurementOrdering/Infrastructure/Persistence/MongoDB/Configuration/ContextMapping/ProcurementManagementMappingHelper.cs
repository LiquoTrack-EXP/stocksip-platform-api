using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace LiquoTrack.StocksipPlatform.API.ProcurementOrdering.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;

public static class ProcurementOrderingMappingHelper
{
    public static void RegisterProcurementOrderingMappings()
    {
        //
        // Register value object serializers
        //

        // PurchaseOrderId
        SerializerRegistrationHelper.TryRegisterSerializer(new PurchaseOrderIdSerializer());

        // CatalogId (shared VO)
        SerializerRegistrationHelper.TryRegisterSerializer(new CatalogIdSerializer());

        // AccountId (shared VO)
        SerializerRegistrationHelper.TryRegisterSerializer(new AccountIdSerializer());

        // ProductId (shared VO)
        SerializerRegistrationHelper.TryRegisterSerializer(new ProductIdSerializer());

        // Enum for order status
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<EOrderStatus>(BsonType.String));

        // PurchaseOrderItem
        SerializerRegistrationHelper.TryRegisterSerializer(new PurchaseOrderItemSerializer());
    }
}