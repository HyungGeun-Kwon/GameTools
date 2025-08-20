using GameTools.Application.Abstractions.Works;
using MediatR;
using GameTools.Application.Abstractions.Stores.WriteStore;

namespace GameTools.Application.Features.Rarities.Commands.DeleteRarity
{
    public sealed class DeleteRarityHandler(IRarityWriteStore rarityWriteStore, IUnitOfWork uow)
        : IRequestHandler<DeleteRarityCommand>
    {
        public async Task Handle(DeleteRarityCommand request, CancellationToken ct)
        {
            var rarity = await rarityWriteStore.GetByIdAsync(request.Id, ct)
                         ?? throw new InvalidOperationException("Rarity not found.");

            // 참조 아이템이 있으면 FK(Restrict)로 DB에서 막힙니다.
            // 사전 확인을 원하면 여기서 Items 존재 여부를 검사하고 친절 메시지로 바꿔주세요.

            rarityWriteStore.Remove(rarity);
            await uow.SaveChangesAsync(ct);
        }
    }
}
