using GameTools.Server.Application.Auditing.Common;

namespace GameTools.Server.Infrastructure.Persistence.Auditing.Models
{
    public abstract class AuditBase
    {
        public Guid AuditId { get; protected set; } = default!;

        public AuditAction Action { get; private set; }
        public DateTime ChangedAtUtc { get; private set; } = DateTime.UtcNow;
        public string ChangedBy { get; private set; } = "unknown";
        public string? BeforeJson { get; private set; }
        public string? AfterJson { get; private set; }
    }
}
