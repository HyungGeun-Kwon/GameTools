namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public enum BulkUpdateStatusCode : byte
    {
        Updated = 0,
        NotFound = 1,
        Concurrency = 2
    }
}
