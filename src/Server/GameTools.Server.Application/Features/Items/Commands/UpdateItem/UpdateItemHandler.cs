using GameTools.Server.Application.Abstractions.Works;
using MediatR;
using GameTools.Server.Application.Abstractions.Stores.WriteStore;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace GameTools.Server.Application.Features.Items.Commands.UpdateItem
{
    public sealed class UpdateItemHandler(IItemWriteStore itemWriteStore, IRarityWriteStore rarityRepo, IUnitOfWork uow)
        : IRequestHandler<UpdateItemCommand, UpdateItemResult>
    {
        public async Task<UpdateItemResult> Handle(UpdateItemCommand request, CancellationToken ct)
        {
            var item = await itemWriteStore.GetByIdAsync(request.Payload.Id, ct);
            if (item == null)
                return new UpdateItemResult(WriteStatusCode.NotFound, null);

            // 감시하고있는  버전 업데이트
            itemWriteStore.SetOriginalRowVersion(item, request.Payload.RowVersion);

            // 변경
            item.SetName(request.Payload.Name);
            item.SetPrice(request.Payload.Price);
            item.SetDescription(request.Payload.Description);

            if (item.RarityId != request.Payload.RarityId)
            {
                var rarity = await rarityRepo.GetByIdAsync(request.Payload.RarityId, ct)
                    ?? throw new InvalidOperationException("Rarity not found.");
                item.SetRarity(rarity);
            }

            try
            {
                await uow.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await itemWriteStore.GetByIdAsync(request.Payload.Id, ct);
                return exists == null ? new(WriteStatusCode.NotFound, null) : new(WriteStatusCode.VersionMismatch, null);
            }

            return new UpdateItemResult
            (
                WriteStatusCode.Success,
                new ItemReadModel(
                    item.Id,
                    item.Name,
                    item.Price,
                    item.Description,
                    item.RarityId,
                    item.Rarity.Grade,
                    item.Rarity.ColorCode,
                    item.RowVersion)
            );
        }
    }
}
