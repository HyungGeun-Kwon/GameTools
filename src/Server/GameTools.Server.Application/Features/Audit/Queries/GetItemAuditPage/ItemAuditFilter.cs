namespace GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage
{
    public sealed record ItemAuditFilter(
        Guid? ItemId,
        IReadOnlyList<string>? Actions,
        DateTime? FromUtc,
        DateTime? ToUtc
    );
}
