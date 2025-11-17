namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed record ItemAuditFilter(int? ItemId, string? Action, DateTime? FromUtc, DateTime? ToUtc);
}
