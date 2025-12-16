using GameTools.Server.Application.Auditing.Common;

namespace GameTools.Server.Application.Auditing.Queries.GetItemAuditPage
{
    public sealed record ItemAuditReadModel(
        Guid Id,
        Guid ItemId,
        AuditAction Action,
        string? BeforeJson,
        string? AfterJson,
        DateTime ChangedAtUtc,
        string ChangedBy
    );
}
