using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTools.Domain.Auditing
{
    public abstract class AuditBase<TKey>
    {
        public TKey AuditId { get; protected set; } = default!;

        public AuditAction Action { get; private set; }
        public string? BeforeJson { get; private set; }
        public string? AfterJson { get; private set; }
        public DateTime ChangedAtUtc { get; private set; } = DateTime.UtcNow;
        public string ChangedBy { get; private set; } = "system";

        protected AuditBase() { }

        protected AuditBase(
            AuditAction action,
            string? beforeJson,
            string? afterJson,
            string changedBy,
            DateTime? changedAtUtc = null)
        {
            if (string.IsNullOrWhiteSpace(changedBy))
                throw new ArgumentException("ChangedBy is required.", nameof(changedBy));

            EnsurePayloadConsistency(action, beforeJson, afterJson);

            Action = action;
            BeforeJson = Normalize(beforeJson);
            AfterJson = Normalize(afterJson);
            ChangedBy = changedBy.Trim();
            ChangedAtUtc = changedAtUtc ?? DateTime.UtcNow;
        }

        protected static void EnsurePayloadConsistency(AuditAction action, string? beforeJson, string? afterJson)
        {
            switch (action)
            {
                case AuditAction.Insert:
                    if (beforeJson is not null || afterJson is null)
                        throw new InvalidOperationException("INSERT requires AfterJson only.");
                    break;
                case AuditAction.Update:
                    if (beforeJson is null || afterJson is null)
                        throw new InvalidOperationException("UPDATE requires both BeforeJson and AfterJson.");
                    break;
                case AuditAction.Delete:
                    if (beforeJson is null || afterJson is not null)
                        throw new InvalidOperationException("DELETE requires BeforeJson only.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action));
            }
        }

        private static string? Normalize(string? json) => string.IsNullOrWhiteSpace(json) ? null : json.Trim();

        protected static string Serialize(object obj, JsonSerializerOptions? options = null)
        {
            ArgumentNullException.ThrowIfNull(obj);
            options ??= DefaultJsonOptions;
            return JsonSerializer.Serialize(obj, options);
        }

        protected static readonly JsonSerializerOptions DefaultJsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
    }
}
