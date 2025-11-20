using FluentValidation.Results;

namespace GameTools.Server.Application.Abstractions.Exceptions
{
    public sealed class ValidationException(IEnumerable<ValidationFailure> failures)
        : AppException("One or more validation errors occurred.")
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; } = failures
                .GroupBy(f => f.PropertyName ?? string.Empty)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.ErrorMessage).ToArray());
    }
}
