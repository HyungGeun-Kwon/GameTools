namespace GameTools.Application.Abstractions.WriteStore
{
    public enum UpdateStatusCode : byte
    {
        Updated = 0,
        NotFound = 1,
        Concurrency = 2
    }
}
