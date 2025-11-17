using GameTools.Server.Domain.Features.Items.Exceptions;

namespace GameTools.Server.Domain.Features.Items.ValueObjects
{
    public sealed record ItemDescription
    {
        public const int MaxLength = 1000;

        public string? Value { get; }

        public ItemDescription(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Value = null;
                return;
            }

            var s = value.Trim();
            if (s.Length > MaxLength)
                throw new ItemDescriptionTooLongException(MaxLength);

            Value = s;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
