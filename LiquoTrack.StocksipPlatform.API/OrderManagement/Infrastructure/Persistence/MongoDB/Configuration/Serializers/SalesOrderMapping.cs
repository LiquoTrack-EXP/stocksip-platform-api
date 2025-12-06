using LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson.Serialization;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Infrastructure.Persistence.MongoDB.Configuration.Serializers;

public class SalesOrderMapping : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder){}
    public static void ConfigureBsonMapping()
    {
        if (BsonClassMap.IsClassMapRegistered(typeof(SalesOrder))) return;
        
        BsonClassMap.RegisterClassMap<SalesOrder>(map =>
        {
            map.AutoMap();
            
            map.MapMember(x => x.OrderCode)
               .SetElementName("orderCode");
               
            map.MapMember(x => x.PurchaseOrderId).SetElementName("purchaseOrderId");
            map.MapMember(x => x.Items).SetElementName("items");
            map.MapMember(x => x.Status).SetElementName("status");
            map.MapMember(x => x.CatalogToBuyFrom).SetElementName("catalogToBuyFrom");
            map.MapMember(x => x.ReceiptDate).SetElementName("receiptDate");
            map.MapMember(x => x.CompletitionDate).SetElementName("completitionDate");
            map.MapMember(x => x.AccountId).SetElementName("buyer");
            map.MapMember(x => x.DeliveryProposal).SetElementName("deliveryProposal");
        });
    }
}
