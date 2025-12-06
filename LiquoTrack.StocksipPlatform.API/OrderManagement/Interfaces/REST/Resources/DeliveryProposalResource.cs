namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Interfaces.REST.Resources;

public record DeliveryProposalResource(
    DateTime ProposedDate,
    string? Notes,
    string Status,
    DateTime CreatedAt,
    DateTime? RespondedAt
);
