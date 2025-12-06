using LiquoTrack.StocksipPlatform.API.InventoryManagement.Domain.Model.Aggregates;
using LiquoTrack.StocksipPlatform.API.Shared.Domain.Model.ValueObjects;

namespace LiquoTrack.StocksipPlatform.Tests.InventoryManagement.Domain.Model.Aggregates;

public class CareGuideTest
{
    [Fact]
    public void Constructor_ValidInputs_ShouldCreateCareGuide()
    {
        var accountId = new AccountId("acc-123");
        var title = "Storage Guide";
        var summary = "How to store properly";
        double minTemp = 2.0;
        double maxTemp = 8.0;
        var place = "Cool and dark place";
        var general = "Keep bottle sealed";

        var careGuide = new CareGuide(accountId, null, title, summary, minTemp, maxTemp, place, general);

        Assert.False(string.IsNullOrWhiteSpace(careGuide.CareGuideId));
        Assert.Equal(accountId, careGuide.AccountId);
        Assert.Null(careGuide.ProductAssociated);
        Assert.Null(careGuide.ProductId);
        Assert.Equal(title, careGuide.Title);
        Assert.Equal(summary, careGuide.Summary);
        Assert.Equal(minTemp, careGuide.RecommendedMinTemperature);
        Assert.Equal(maxTemp, careGuide.RecommendedMaxTemperature);
        Assert.Equal(place, careGuide.RecommendedPlaceStorage);
        Assert.Equal(general, careGuide.GeneralRecommendation);
    }
}
