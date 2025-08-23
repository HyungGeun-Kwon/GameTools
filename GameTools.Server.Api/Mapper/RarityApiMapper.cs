using GameTools.Contracts.Rarities.Common;
using GameTools.Contracts.Rarities.CreateRarity;
using GameTools.Contracts.Rarities.DeleteRarity;
using GameTools.Contracts.Rarities.GetAllRarities;
using GameTools.Contracts.Rarities.UpdateRarity;
using GameTools.Server.Api.Extensions;
using GameTools.Server.Application.Features.Rarities.Commands.CreateRarity;
using GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity;
using GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity;
using GameTools.Server.Application.Features.Rarities.Models;

namespace GameTools.Server.Api.Mapper
{
    public static class RarityApiMapper
    {
        public static RarityResponse ToResponse(this RarityReadModel readModel)
            => new(readModel.Id, readModel.Grade, readModel.ColorCode, readModel.RowVersion.ToBase64RowVersion());

        public static AllRarityResponse ToResponse(this IReadOnlyList<RarityReadModel> list)
            => new(list.Select(ToResponse).ToList());

        public static CreateRarityPayload ToPayload(this CreateRarityRequest req)
            => new(req.Grade, req.ColorCode);

        public static UpdateRarityPayload ToPayload(this UpdateRarityRequest req)
            => new(req.Id, req.Grade, req.ColorCode, Convert.FromBase64String(req.RowVersionBase64));

        public static DeleteRarityPayload ToPayload(this DeleteRarityRequest req)
            => new(req.Id, Convert.FromBase64String(req.RowVersionBase64));
    }
}
