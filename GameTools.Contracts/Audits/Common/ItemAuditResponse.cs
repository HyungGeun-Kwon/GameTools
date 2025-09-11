namespace GameTools.Contracts.Audits.Common
{
    public sealed record ItemAuditResponse(long AuditId, int ItemId, string Action, string? BeforeJson, string? AfterJson, DateTime ChangedAtUtc, string ChangedBy);
}
