namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public enum BulkDeleteStatusCode : byte
    {
        Deleted = 0,
        NotFound = 1,
        Concurrency = 2,
    }
}
