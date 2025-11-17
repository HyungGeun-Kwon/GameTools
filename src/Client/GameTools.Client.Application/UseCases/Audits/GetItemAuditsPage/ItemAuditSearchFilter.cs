namespace GameTools.Client.Application.UseCases.Audits.GetItemAuditsPage
{
    public sealed record ItemAuditSearchFilter(int? ItemId, string? Action, DateTime? FromUtc, DateTime? ToUtc);
}
