namespace GameTools.Client.Application.UseCases.Restores.GetRestoreHistoryPage
{
    public sealed record RestoreHistorySearchFilter(DateTime? FromUtc, DateTime? ToUtc, string? Actor, bool? DryOnly);
}
