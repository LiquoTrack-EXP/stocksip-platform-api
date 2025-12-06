using System.ComponentModel.DataAnnotations;

namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public record RespondDeliveryProposalRequest(
    [Required] bool Accept,
    string? Notes
);
