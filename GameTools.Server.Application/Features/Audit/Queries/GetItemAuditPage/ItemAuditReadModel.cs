namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed record ItemAuditReadModel(
        long? AuditId,
        int ItemId,
        string Action,
        string? BeforeJson,
        string? AfterJson,
        DateTime ChangedAtUtc,
        string ChangedBy
    );
}
