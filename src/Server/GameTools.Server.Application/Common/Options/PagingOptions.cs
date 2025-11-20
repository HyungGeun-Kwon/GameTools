namespace GameTools.Server.Application.Common.Options
{
    public sealed class PagingOptions
    {
        public int DefaultPageSize { get; init; } = 20;
        public int MaxPageSize { get; init; } = 100;
    }
}
