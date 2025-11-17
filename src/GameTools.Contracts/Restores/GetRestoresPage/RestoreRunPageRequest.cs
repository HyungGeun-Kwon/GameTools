namespace GameTools.Contracts.Restores.GetRestoresPage
{
    public sealed record RestoreRunPageRequest(
        int PageNumber = 1,
        int PageSize = 50,
        DateTime? FromUtc = null,
        DateTime? ToUtc = null,
        string? Actor = null,
        bool? DryOnly = null);
}
