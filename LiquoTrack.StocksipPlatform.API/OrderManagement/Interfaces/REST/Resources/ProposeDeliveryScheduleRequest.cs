using System.ComponentModel.DataAnnotations;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public record ProposeDeliveryScheduleRequest(
    [Required] DateTime ProposedDate,
    string? Notes
);
