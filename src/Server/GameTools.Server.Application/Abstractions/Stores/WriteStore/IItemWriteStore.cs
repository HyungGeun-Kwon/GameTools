using GameTools.Server.Application.Features.Items.Commands.Common.Bulk;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;
using GameTools.Server.Domain.Features.Items.Entities;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public interface IItemWriteStore : IWriteStore<Item, ItemId>
    {
        /// <summary>
        /// Executes a bulk insert in a single roundtrip (e.g. TVP).
        /// This method manages its own transaction / SaveChanges internally.
        /// </summary>
        Task<IReadOnlyList<BulkResultRow>> BulkInsertAsync(
            IReadOnlyList<CreateItemSpec> items,
            CancellationToken ct);

        /// <summary>
        /// Executes a bulk update in a single roundtrip (e.g. TVP).
        /// This method manages its own transaction / SaveChanges internally.
        /// </summary>
        Task<IReadOnlyList<BulkResultRow>> BulkUpdateAsync(
            IReadOnlyList<UpdateItemSpec> items,
            CancellationToken ct);

        /// <summary>
        /// Executes a bulk delete in a single roundtrip (e.g. TVP).
        /// This method manages its own transaction / SaveChanges internally.
        /// </summary>
        Task<IReadOnlyList<BulkResultRow>> BulkDeleteAsync(
            IReadOnlyList<DeleteItemSpec> items,
            CancellationToken ct);
    }
}
