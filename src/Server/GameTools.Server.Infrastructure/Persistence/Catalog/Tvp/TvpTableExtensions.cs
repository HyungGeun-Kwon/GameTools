using System.Data;
using GameTools.Server.Application.Catalog.Items.Commands.Common.Specs;

namespace GameTools.Server.Infrastructure.Persistence.Catalog.Tvp
{
    public static class TvpTableExtensions
    {
        public static DataTable CreateItemInsertDataTable(this IEnumerable<CreateItemSpec> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("RarityId", typeof(byte));

            foreach (var r in rows)
                dt.Rows.Add(r.Name, r.Price, r.Description ?? (object)DBNull.Value, r.RarityId);

            return dt;
        }

        public static DataTable CreateItemUpdateDataTable(this IEnumerable<UpdateItemSpec> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(Guid));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("RarityId", typeof(byte));
            dt.Columns.Add("RowVersionOriginal", typeof(byte[]));

            foreach (var r in rows)
                dt.Rows.Add(
                    r.Id,
                    r.Name,
                    r.Price,
                    r.Description ?? (object)DBNull.Value,
                    r.RarityId,
                    r.RowVersion);

            return dt;
        }

        public static DataTable CreateItemDeleteDataTable(this IEnumerable<DeleteItemSpec> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("RowVersionOriginal", typeof(byte[]));

            foreach (var r in rows)
                dt.Rows.Add(r.Id, r.RowVersion);

            return dt;
        }
    }
}
