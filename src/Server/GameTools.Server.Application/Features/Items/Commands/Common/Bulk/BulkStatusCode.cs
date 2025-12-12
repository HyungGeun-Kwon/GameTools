namespace GameTools.Server.Application.Features.Items.Commands.Common.Bulk
{
    public enum BulkStatusCode : byte
    {
        Succeeded = 0,
        NotFound = 1,
        Concurrency = 2,
        ValidationFailed = 3, // InvalidPrice, InvalidRarity 등 묶음
        Conflict = 4, // DuplicateName 등 유니크 충돌
        UnknownError = 5 // 기타 미분류 오류
    }
}
