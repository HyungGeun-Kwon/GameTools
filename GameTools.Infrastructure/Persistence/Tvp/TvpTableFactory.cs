using System.Data;
using GameTools.Application.Features.Items.Dtos;

namespace GameTools.Infrastructure.Persistence.Tvp
{
    public static class TvpTableFactory
    {
        public static DataTable CreateItemInsertDataTable(IEnumerable<ItemCreateDto> itemCreateDtos)
        {
            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("RarityId", typeof(byte));

            foreach (var r in itemCreateDtos)
                dt.Rows.Add(r.Name, r.Price, r.Description ?? (object)DBNull.Value, r.RarityId);

            return dt;
        }

        public static DataTable CreateItemUpdateDataTable(IEnumerable<ItemUpdateDto> itemUpdateDtos)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(int));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("RarityId", typeof(byte));
            dt.Columns.Add("RowVersionOriginal", typeof(byte[]));

            foreach (var r in itemUpdateDtos)
                dt.Rows.Add(r.Id, r.Name, r.Price, r.Description ?? (object)DBNull.Value, r.RarityId);

            return dt;
        }
    }
}
