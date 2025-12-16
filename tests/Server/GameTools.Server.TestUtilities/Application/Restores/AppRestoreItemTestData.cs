using GameTools.Server.Application.Operations.Restores.Commands.RestoreItems;

namespace GameTools.Server.TestUtilities.Application.Restores
{
    public static  class AppRestoreItemTestData
    {
        public static RestoreItemsSpec BuildDefaultRestoreItemsSpec(
            DateTime? asOfUtc = null,
            IReadOnlyCollection<Guid>? itemIds = null,
            string? notes = null,
            bool? dryRun = null
            )
            => new(
                AsOfUtc: asOfUtc ?? DateTime.UtcNow.AddMinutes(-1),
                ItemIds: itemIds,
                Notes: notes ?? "note",
                DryRun: dryRun ?? false);
    }
}
