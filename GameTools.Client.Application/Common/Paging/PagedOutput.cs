namespace GameTools.Client.Application.Common.Paging
{
    public sealed record PagedOutput<T>(IReadOnlyList<T> Items, int TotalCount, int PageNumber, int PageSize);
}
