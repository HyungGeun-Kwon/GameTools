using System.Net.Mime;
using GameTools.Contracts.Rarities.Common;
using GameTools.Contracts.Rarities.CreateRarity;
using GameTools.Contracts.Rarities.DeleteRarity;
using GameTools.Contracts.Rarities.GetAllRarities;
using GameTools.Contracts.Rarities.UpdateRarity;
using GameTools.Server.Api.Mapper;
using GameTools.Server.Application.Features.Rarities.Commands.CreateRarity;
using GameTools.Server.Application.Features.Rarities.Commands.DeleteRarity;
using GameTools.Server.Application.Features.Rarities.Commands.UpdateRarity;
using GameTools.Server.Application.Features.Rarities.Queries.GetRarities;
using GameTools.Server.Application.Features.Rarities.Queries.GetRarityById;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace GameTools.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Tags("Rarities")]
    public sealed class RaritiesController(IMediator mediator) : ControllerBase
    {
        // ID기반 조회
        [HttpGet("{id:byte}")]
        public async Task<ActionResult<RarityResponse>> Get(
            [FromRoute] byte id, CancellationToken ct = default)
        {
            var read = await mediator.Send(new GetRarityByIdQuery(id), ct);
            return read is null ? NotFound() : Ok(read.ToResponse());
        }

        // 전체 조회
        [HttpGet]
        public async Task<ActionResult<AllRarityResponse>> GetAll(CancellationToken ct)
        {
            var reads = await mediator.Send(new GetRaritiesQuery(), ct);
            return Ok(reads.ToResponse());
        }

        // 생성
        [HttpPost]
        public async Task<ActionResult<RarityResponse>> Create(
            [FromBody] CreateRarityRequest request, CancellationToken ct)
        {
            var read = await mediator.Send(new CreateRarityCommand(request.ToPayload()), ct);
            return Ok(read.ToResponse());
        }

        [HttpDelete]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Delete(
            [FromBody] DeleteRarityRequest request, CancellationToken ct)
        {
            await mediator.Send(new DeleteRarityCommand(request.ToPayload()), ct);
            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult<RarityResponse>> Update(
            [FromBody] UpdateRarityRequest request, CancellationToken ct)
        {
            var read = await mediator.Send(new UpdateRarityCommand(request.ToPayload()), ct);
            return read is null ? NotFound() : Ok(read.ToResponse());
        }
    }
}
