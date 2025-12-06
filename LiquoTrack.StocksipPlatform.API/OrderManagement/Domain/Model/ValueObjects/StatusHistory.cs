namespace LiquoTrack.StocksipPlatform.API.OrderManagement.Domain.Model.ValueObjects
{
    /// <summary>
    /// Represents a status change in the order's lifecycle
    /// </summary>
    public record StatusHistory
    {
        public ESalesOrderStatuses Status { get; init; }
        public DateTime Timestamp { get; init; }
        public string ChangedBy { get; init; }
        public string Reason { get; init; }

        public StatusHistory(ESalesOrderStatuses status, string changedBy, string reason = null)
        {
            Status = status;
            Timestamp = DateTime.UtcNow;
            ChangedBy = changedBy ?? "System";
            Reason = reason ?? string.Empty;
        }
        private StatusHistory() { }
    }
}
