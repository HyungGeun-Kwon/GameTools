using System.Net.Mime;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.BulkDeleteItems;
using GameTools.Contracts.Items.BulkInsertItems;
using GameTools.Contracts.Items.BulkUpdateItems;
using GameTools.Contracts.Items.Common;
using GameTools.Contracts.Items.CreateItem;
using GameTools.Contracts.Items.GetItemPage;
using GameTools.Contracts.Items.GetItemsByRarity;
using GameTools.Contracts.Items.RestoreItemsAsOf;
using GameTools.Contracts.Items.UpdateItem;
using GameTools.Server.Api.Mapper;
using GameTools.Server.Application.Common.Results;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Commands.DeleteItem;
using GameTools.Server.Application.Features.Items.Commands.DeleteItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.RestoreItemsAsOf;
using GameTools.Server.Application.Features.Items.Commands.UpdateItem;
using GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemById;
using GameTools.Server.Application.Features.Items.Queries.GetItemPage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameTools.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Tags("Items")]
    public sealed class ItemsController(IMediator mediator) : ControllerBase
    {
        // ID기반 조회
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemResponse>> Get(
            [FromRoute] int id, CancellationToken ct = default)
        {
            ItemReadModel? read = await mediator.Send(new GetItemByIdQuery(id), ct);
            return read is null ? NotFound() : Ok(read.ToResponse());
        }

        // Page기반 조회
        [HttpPost("page-search")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResponse<ItemResponse>>> GetPage(
            [FromBody] ItemsPageRequest request, CancellationToken ct)
        {
            var criteria = request.ToCriteria();
            var query = new GetItemsPageQuery(criteria);
            var page = await mediator.Send(query, ct);
            return Ok(page.ToResponse());
        }

        // Rarity기반 조회
        [HttpGet("by-rarity/{rarityId:int:range(0,255)}")]
        public async Task<ActionResult<ItemsByRarityResponse>> GetByRarity(
            [FromRoute] int rarityId, CancellationToken ct = default)
        {
            var reads = await mediator.Send(new GetItemsByRarityIdQuery((byte)rarityId), ct);
            var response = reads.ToResponse();
            return Ok(response);
        }

        // 생성
        [HttpPost]
        public async Task<ActionResult<ItemResponse>> Create(
            [FromBody] CreateItemRequest request, CancellationToken ct)
        {
            var read = await mediator.Send(new CreateItemCommand(request.ToPayload()), ct);
            return Ok(read.ToResponse());
        }

        // 삭제
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            [FromRoute] int id, CancellationToken ct)
        {
            if (!Request.Headers.TryGetValue("If-Match", out var ifMatch) || string.IsNullOrWhiteSpace(ifMatch))
                return Problem(statusCode: StatusCodes.Status428PreconditionRequired, title: "If-Match header required");

            var etag = ifMatch.ToString().Trim('"');
            byte[] rowVersion;
            try { rowVersion = Convert.FromBase64String(etag); }
            catch { return Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid ETag"); }

            var payload = new DeleteItemPayload(id, rowVersion);
            WriteStatusCode statusCode = await mediator.Send(new DeleteItemCommand(payload), ct);

            return statusCode switch
            {
                WriteStatusCode.Success => NoContent(),
                WriteStatusCode.NotFound => NotFound(),
                WriteStatusCode.VersionMismatch => StatusCode(StatusCodes.Status412PreconditionFailed),
                _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
            };
        }

        // 업데이트
        [HttpPut]
        public async Task<ActionResult<ItemResponse>> Update(
            [FromBody] UpdateItemRequest request,
            CancellationToken ct)
        {
            var read = await mediator.Send(new UpdateItemCommand(request.ToPayload()), ct);

            if (read.WriteStatusCode == WriteStatusCode.NotFound)
                return NotFound();
            else if (read.WriteStatusCode == WriteStatusCode.VersionMismatch)
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            else if (read.WriteStatusCode == WriteStatusCode.Success && read.ItemReadModel is not null)
                return Ok(read.ItemReadModel.ToResponse());

            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        // 벌크 삽입 (클래스 단위 Body)
        [HttpPost("bulk-insert")]
        public async Task<ActionResult<BulkInsertItemsResponse>> BulkInsert(
            [FromBody] BulkInsertItemsRequest req, CancellationToken ct)
        {
            var reads = await mediator.Send(new InsertItemsTvpCommand(req.ToRows()), ct);
            return Ok(reads.ToResponse());
        }

        // 벌크 업데이트 (클래스 단위 Body)
        [HttpPut("bulk-update")]
        public async Task<ActionResult<BulkUpdateItemsResponse>> BulkUpdate(
            [FromBody] BulkUpdateItemsRequest req, CancellationToken ct)
        {
            var reads = await mediator.Send(new UpdateItemsTvpCommand(req.ToRows()), ct);
            return Ok(reads.ToResponse());
        }

        // 벌크 삭제 (클래스 단위 Body)
        [HttpPut("bulk-delete")]
        public async Task<ActionResult<BulkDeleteItemsResponse>> BulkDelete(
            [FromBody] BulkDeleteItemsRequest req, CancellationToken ct)
        {
            var reads = await mediator.Send(new DeleteItemsTvpCommand(req.ToRows()), ct);
            return Ok(reads.ToResponse());
        }

        // 복원
        [HttpPost("restore-asof")]
        public async Task<ActionResult<object>> RestoreAsOf(
            [FromBody] RestoreItemsAsOfRequest req, 
            CancellationToken ct)
        {
            var result = await mediator.Send(new RestoreItemsAsOfCommand(req.ToPayload()), ct);
            return Ok(result.ToResponse());
        }
    }
}
