using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;
using LiquoTrack.StocksipPlatform.API.OrderManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using LiquoTrack.StocksipPlatform.API.Shared.Infrastructure.Persistence.MongoDB.Configuration.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Infrastructure.Persistence.MongoDB.Configuration.ContextMapping;

public static class SalesOrderMappingHelper
{
    public static void RegisterSalesOrderManagementMappings()
    {
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<ESalesOrderStatuses>(BsonType.String));
        SerializerRegistrationHelper.TryRegisterSerializer(new EnumSerializer<DeliveryProposalStatus>(BsonType.String));
        SalesOrderMapping.ConfigureBsonMapping();
    }
}