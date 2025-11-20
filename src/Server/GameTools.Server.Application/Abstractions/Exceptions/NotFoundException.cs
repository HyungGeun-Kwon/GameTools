namespace GameTools.Server.Application.Abstractions.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
