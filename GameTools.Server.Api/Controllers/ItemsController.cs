using System.Net.Mime;
using GameTools.Server.Api.Mapper;
using GameTools.Server.Application.Features.Items.Commands.CreateItem;
using GameTools.Server.Application.Features.Items.Commands.DeleteItem;
using GameTools.Server.Application.Features.Items.Commands.InsertItemsTvp;
using GameTools.Server.Application.Features.Items.Commands.UpdateItem;
using GameTools.Server.Application.Features.Items.Commands.UpdateItemsTvp;
using GameTools.Server.Application.Features.Items.Models;
using GameTools.Server.Application.Features.Items.Queries.GetItemById;
using GameTools.Server.Application.Features.Items.Queries.GetItemPage;
using GameTools.Contracts.Items.BulkUpdateItems;
using GameTools.Contracts.Items.Common;
using GameTools.Contracts.Items.CreateItem;
using GameTools.Contracts.Items.DeleteItem;
using GameTools.Contracts.Items.GetItemPage;
using GameTools.Contracts.Items.GetItemsByRarity;
using GameTools.Contracts.Items.UpdateItem;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.BulkInsertItems;

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
        [HttpDelete]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> Delete(
            [FromBody] DeleteItemRequest request, CancellationToken ct)
        {
            await mediator.Send(new DeleteItemCommand(request.ToPayload()), ct);
            return NoContent();
        }

        // 업데이트
        [HttpPut]
        public async Task<ActionResult<ItemResponse>> Update(
            [FromBody] UpdateItemRequest request,
            CancellationToken ct)
        {
            var read = await mediator.Send(new UpdateItemCommand(request.ToPayload()), ct);
            return read is null ? NotFound() : Ok(read.ToResponse());
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
    }
}
