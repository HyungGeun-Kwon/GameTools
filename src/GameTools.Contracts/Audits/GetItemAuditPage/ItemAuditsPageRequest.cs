namespace GameTools.Contracts.Items.GetItemAuditPage
{
    public sealed record ItemAuditsPageRequest(
        int PageNumber = 1,
        int PageSize = 20,
        int? ItemId = null,
        string? Action = null, // INSERT/UPDATE/DELETE
        DateTime? FromUtc = null,
        DateTime? ToUtc = null);
}
