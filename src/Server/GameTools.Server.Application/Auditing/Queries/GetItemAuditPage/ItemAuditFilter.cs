namespace GameTools.Server.Application.Auditing.Queries.GetItemAuditPage
{
    public sealed record ItemAuditFilter(
        Guid? ItemId,
        IReadOnlyList<string>? Actions,
        DateTime? FromUtc,
        DateTime? ToUtc
    );
}
