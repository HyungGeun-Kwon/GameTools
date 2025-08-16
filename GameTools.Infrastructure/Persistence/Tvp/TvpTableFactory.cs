using System.Data;
using GameTools.Application.Features.Items.Commands.Common;

namespace GameTools.Infrastructure.Persistence.Tvp
{
    public static class TvpTableFactory
    {
        public static DataTable CreateItemInsertDataTable(IEnumerable<ItemInsertRow> rows)
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

        public static DataTable CreateItemUpdateDataTable(IEnumerable<ItemUpdateRow> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("RarityId", typeof(byte));
            dt.Columns.Add("RowVersionOriginal", typeof(byte[]));

            foreach (var r in rows)
                dt.Rows.Add(r.Id, r.Name, r.Price, r.Description ?? (object)DBNull.Value, r.RarityId);

            return dt;
        }
    }
}
