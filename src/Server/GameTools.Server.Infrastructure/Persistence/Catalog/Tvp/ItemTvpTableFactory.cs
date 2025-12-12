using System.Data;
using GameTools.Server.Application.Features.Items.Commands.Common.Specs;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Tvp
{
    internal static class ItemTvpTableFactory
    {
        public static DataTable CreateInsertTable(IReadOnlyList<CreateItemSpec> items)
        {
            var table = new DataTable();
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Price", typeof(int));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("RarityId", typeof(Guid));

            for (var i = 0; i < items.Count; i++)
            {
                var it = items[i];
                table.Rows.Add(i, it.Name, it.Price, (object?)it.Description ?? DBNull.Value, it.RarityId);
            }

            return table;
        }

        public static DataTable CreateUpdateTable(IReadOnlyList<UpdateItemSpec> items)
        {
            var table = new DataTable();
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Price", typeof(int));
            table.Columns.Add("Description", typeof(string));
            table.Columns.Add("RarityId", typeof(Guid));
            table.Columns.Add("RowVersionOriginal", typeof(byte[]));

            for (var i = 0; i < items.Count; i++)
            {
                var it = items[i];
                table.Rows.Add(i, it.Id, it.Name, it.Price, (object?)it.Description ?? DBNull.Value, it.RarityId, it.RowVersion);
            }

            return table;
        }

        public static DataTable CreateDeleteTable(IReadOnlyList<DeleteItemSpec> items)
        {
            var table = new DataTable();
            table.Columns.Add("Index", typeof(int));
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("RowVersionOriginal", typeof(byte[]));

            for (var i = 0; i < items.Count; i++)
            {
                var it = items[i];
                table.Rows.Add(i, it.Id, it.RowVersion);
            }

            return table;
        }
    }
}
