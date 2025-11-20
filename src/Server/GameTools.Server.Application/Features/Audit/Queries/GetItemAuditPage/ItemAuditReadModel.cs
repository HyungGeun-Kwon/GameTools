namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed record ItemAuditReadModel(
        Guid Id,
        Guid ItemId,
        string Action,
        string? BeforeJson,
        string? AfterJson,
        DateTime ChangedAtUtc,
        string ChangedBy
    );
}
