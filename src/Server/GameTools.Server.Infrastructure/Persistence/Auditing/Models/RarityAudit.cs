namespace GameTools.Server.Infrastructure.Persistence.Auditing.Models
{
    public class RarityAudit : AuditBase
    {
        public Guid RarityId { get; private set; }

        private RarityAudit() { } // EF Core
    }
}
