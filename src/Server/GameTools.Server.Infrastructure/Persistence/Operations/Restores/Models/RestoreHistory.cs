namespace GameTools.Server.Infrastructure.Persistence.Operations.Restores.Models
{
    public sealed class RestoreHistory
    {
        public Guid RestoreId { get; set; }
        public DateTime AsOfUtc { get; set; }
        public string Actor { get; set; } = "unknown";
        public bool DryRun { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? EndedAtUtc { get; set; }
        public string? AffectedCounts { get; set; }
        public string? Notes { get; set; }
        public string? FiltersJson { get; set; }
    }
}
