using GameTools.Domain.Entities;

namespace GameTools.Application.Abstractions.WriteStore
{
    public interface IItemWriteStore : IWriteStore<Item, int>
    {
    }
}
