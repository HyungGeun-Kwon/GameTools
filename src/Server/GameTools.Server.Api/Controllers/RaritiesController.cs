using System.Net.Mime;
using GameTools.Contracts.Rarities.Common;
using GameTools.Contracts.Rarities.CreateRarity;
using GameTools.Contracts.Rarities.GetAllRarities;
using GameTools.Contracts.Rarities.UpdateRarity;
using GameTools.Server.Api.Mapper;
using GameTools.Server.Application.Common.Results;
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
        [HttpGet("{id:int:range(0,255)}")]
        public async Task<ActionResult<RarityResponse>> Get(
            [FromRoute] int id, CancellationToken ct = default)
        {
            var read = await mediator.Send(new GetRarityByIdQuery((byte)id), ct);
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

        [HttpDelete("{id:int:range(0,255)}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id, CancellationToken ct)
        {
            if (!Request.Headers.TryGetValue("If-Match", out var ifMatch) || string.IsNullOrWhiteSpace(ifMatch))
                return Problem(statusCode: StatusCodes.Status428PreconditionRequired, title: "If-Match header required");

            var etag = ifMatch.ToString().Trim('"');
            byte[] rowVersion;
            try { rowVersion = Convert.FromBase64String(etag); }
            catch { return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid ETag"); }

            var payload = new DeleteRarityPayload((byte)id, rowVersion);

            WriteStatusCode statusCode = await mediator.Send(new DeleteRarityCommand(payload), ct);

            return statusCode switch
            {
                WriteStatusCode.Success => NoContent(),
                WriteStatusCode.NotFound => NotFound(),
                WriteStatusCode.VersionMismatch => StatusCode(StatusCodes.Status412PreconditionFailed),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        [HttpPut]
        public async Task<ActionResult<RarityResponse>> Update(
            [FromBody] UpdateRarityRequest request, CancellationToken ct)
        {
            var read = await mediator.Send(new UpdateRarityCommand(request.ToPayload()), ct);


            if (read.WriteStatusCode == WriteStatusCode.NotFound)
                return NotFound();
            else if (read.WriteStatusCode == WriteStatusCode.VersionMismatch)
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            else if (read.WriteStatusCode == WriteStatusCode.Success && read.RarityReadModel is not null)
                return Ok(read.RarityReadModel.ToResponse());

            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
