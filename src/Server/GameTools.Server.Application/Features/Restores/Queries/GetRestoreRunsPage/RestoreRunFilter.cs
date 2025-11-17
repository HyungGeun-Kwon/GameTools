namespace GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage
{
    public sealed record RestoreRunFilter(DateTime? FromUtc, DateTime? ToUtc, string? Actor, bool? DryOnly);
}
