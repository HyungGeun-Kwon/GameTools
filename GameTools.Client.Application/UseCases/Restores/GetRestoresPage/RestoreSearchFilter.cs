namespace GameTools.Client.Application.UseCases.Restores.GetRestoresPage
{
    public sealed record RestoreSearchFilter(DateTime? FromUtc, DateTime? ToUtc, string? Actor, bool? DryOnly);
}
