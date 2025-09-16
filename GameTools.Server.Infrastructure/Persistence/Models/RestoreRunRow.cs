namespace GameTools.Server.Infrastructure.Persistence.Models
{
    public sealed class RestoreRunRow
    {
        public Guid RestoreId { get; set; }
        public DateTime AsOfUtc { get; set; }
        public string Actor { get; set; } = "";
        public bool DryRun { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? EndedAtUtc { get; set; }
        public string? AffectedCounts { get; set; }
        public string? Notes { get; set; }
        public string? FiltersJson { get; set; }
    }
}
