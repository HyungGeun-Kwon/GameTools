using GameTools.Server.Domain.Common.Checkers;
using GameTools.Server.Domain.Features.Items.ValueObjects;

namespace GameTools.Server.Domain.Features.Items.Services
{
    public interface IItemNameUniquenessChecker : IUniquenessChecker<ItemName>;
}
