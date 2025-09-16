namespace GameTools.Client.Application.Models.Audits
{
    public sealed record ItemAudit(long AuditId, string Action, string? BeforeJson, string? AfterJson, DateTime ChangedAtUtc, string ChangedBy);
}
