namespace GameTools.Server.Application.Features.Items.Commands.Common.Bulk
{
    public sealed record BulkResultRow(
        int Index,
        Guid Id,
        BulkStatusCode Status,
        string? ErrorCode = null,
        string? ErrorMessage = null);
}
