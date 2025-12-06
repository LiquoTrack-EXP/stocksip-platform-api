namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects;

public class DeliveryProposal
{
    public DateTime ProposedDate { get; set; }
    public string? Notes { get; set; }
    public DeliveryProposalStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}
