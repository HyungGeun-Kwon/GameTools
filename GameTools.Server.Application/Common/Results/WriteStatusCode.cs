namespace GameTools.Server.Application.Common.Results
{
    public enum WriteStatusCode : byte
    { 
        Success = 0, 
        NotFound = 1, 
        VersionMismatch = 2
    }
}
