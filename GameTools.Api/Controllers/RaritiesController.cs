//using GameTools.Application.Features.Rarities.Commands.CreateRarity;
//using GameTools.Application.Features.Rarities.Commands.DeleteRarity;
//using GameTools.Application.Features.Rarities.Commands.UpdateRarity;
//using GameTools.Application.Features.Rarities.Dtos;
//using GameTools.Application.Features.Rarities.Queries.GetRarities;
//using GameTools.Application.Features.Rarities.Queries.GetRarityById;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;


//namespace GameTools.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public sealed class RaritiesController(IMediator mediator) : ControllerBase
//    {
//        [HttpGet]
//        public async Task<ActionResult<List<RarityDto>>> GetAll(CancellationToken ct)
//            => await mediator.Send(new GetRaritiesQuery(), ct);

//        [HttpGet("{id:byte}")]
//        public async Task<ActionResult<RarityDto>> GetById(byte id, CancellationToken ct)
//        {
//            var dto = await mediator.Send(new GetRarityByIdQuery(id), ct);
//            return dto is null ? NotFound() : Ok(dto);
//        }

//        [HttpPost]
//        public async Task<ActionResult<RarityDto>> Create([FromBody] CreateRarityCommand cmd, CancellationToken ct)
//        {
//            var dto = await mediator.Send(cmd, ct);
//            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
//        }

//        // 경로 id와 Body(UpdateRarityCommand)의 Id를 맞춰서 보냄
//        [HttpPut("{id:byte}")]
//        public async Task<ActionResult<RarityDto>> Update(byte id, [FromBody] UpdateRarityCommand body, CancellationToken ct)
//            => await mediator.Send(body with { Id = id }, ct);

//        [HttpDelete("{id:byte}")]
//        public async Task<IActionResult> Delete(byte id, CancellationToken ct)
//        {
//            await mediator.Send(new DeleteRarityCommand(id), ct);
//            return NoContent();
//        }
//    }
//}
