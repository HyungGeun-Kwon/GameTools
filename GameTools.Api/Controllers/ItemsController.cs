//using GameTools.Application.Common.Paging;
//using GameTools.Application.Features.Items.Commands.CreateItem;
//using GameTools.Application.Features.Items.Commands.DeleteItem;
//using GameTools.Application.Features.Items.Commands.InsertItemsTvp;
//using GameTools.Application.Features.Items.Commands.UpdateItem;
//using GameTools.Application.Features.Items.Commands.UpdateItemsTvp;
//using GameTools.Application.Features.Items.Dtos;
//using GameTools.Application.Features.Items.Queries.GetItemById;
//using GameTools.Application.Features.Items.Queries.GetItemPage;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace GameTools.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public sealed class ItemsController(IMediator mediator) : ControllerBase
//    {
//        [HttpGet("{id:int}")]
//        public async Task<ActionResult<ItemDto>> Get(int id, CancellationToken ct)
//        {
//            var dto = await mediator.Send(new GetItemByIdQuery(id), ct);
//            return dto is null ? NotFound() : Ok(dto);
//        }

//        [HttpGet]
//        public async Task<ActionResult<PagedResult<ItemDto>>> GetPage(
//            [FromQuery] int page = 1,
//            [FromQuery] int size = 20,
//            [FromQuery] string? search = null,
//            [FromQuery] byte? rarityId = null,
//            CancellationToken ct = default)
//            => await mediator.Send(new GetItemsPageQuery(page, size, search, rarityId), ct);

//        [HttpPost]
//        public async Task<ActionResult<ItemDto>> Create([FromBody] CreateItemCommand cmd, CancellationToken ct)
//        {
//            ItemDto dto = await mediator.Send(cmd, ct);
//            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
//        }

//        [HttpPut("{id:int}")]
//        public async Task<ActionResult<ItemDto>> Update(int id, [FromBody] UpdateItemCommand body, CancellationToken ct)
//            => await mediator.Send(body with { Id = id }, ct);

//        [HttpDelete("{id:int}")]
//        public async Task<IActionResult> Delete(int id, CancellationToken ct)
//        {
//            await mediator.Send(new DeleteItemCommand(id), ct);
//            return NoContent();
//        }

//        // TVP
//        [HttpPost("tvp/insert")]
//        public async Task<ActionResult<IReadOnlyList<InsertedItemsTvpResult>>> InsertManyTvp(
//            [FromBody] InsertItemsTvpCommand cmd, CancellationToken ct)
//        {
//            IReadOnlyList<InsertedItemsTvpResult> result = await mediator.Send(cmd, ct);
//            return Ok(result);
//        }

//        [HttpPost("tvp/update")]
//        public async Task<ActionResult<IReadOnlyList<UpdatedItemsTvpResult>>> UpdateManyTvp(
//            [FromBody] UpdateItemsTvpCommand cmd, CancellationToken ct)
//        {
//            IReadOnlyList<UpdatedItemsTvpResult> result = await mediator.Send(cmd, ct);
//            return Ok(result);
//        }
//    }
//}
