namespace GameTools.Server.Infrastructure.Persistence.Auditing.Models
{
    public class ItemAudit : AuditBase
    {
        public Guid ItemId { get; private set; }

        private ItemAudit() { } // EF Core
    }
}
