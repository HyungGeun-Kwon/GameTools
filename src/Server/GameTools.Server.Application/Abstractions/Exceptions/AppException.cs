namespace GameTools.Server.Application.Abstractions.Exceptions
{
    // TODO : API 에러 매핑 (예정)
    // NotFoundException → 404
    // ConflictException(RowVersion 불일치, 유니크 키/인덱스 위반) → 409
    // DomainException/ValidationException → 400/422
    // 기타 미처리 → 500
    
    // 필요시 ErrorCode/메타데이터 추가 고려.
    public abstract class AppException : Exception
    {
        protected AppException(string message) : base(message) { }
        protected AppException(string message, Exception innerException) : base(message, innerException) { }
    }
}
