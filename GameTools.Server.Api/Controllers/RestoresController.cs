using System.Net.Mime;
using GameTools.Contracts.Common;
using GameTools.Contracts.Restores.GetRestoresPage;
using GameTools.Server.Api.Mapper;
using GameTools.Server.Application.Features.Restores.Queries.GetRestoreRunsPage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameTools.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Tags("Restores")]
    public class RestoresController(IMediator mediator) : ControllerBase
    {
        [HttpPost("page-search")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResponse<RestoreRunReadModel>>> GetRestoreRuns(
            [FromBody] RestoreRunPageRequest request, CancellationToken ct = default)
        {
            var criteria = request.ToCriteria();

            var query = new GetRestoreRunsPageQuery(criteria);
            var page = await mediator.Send(query, ct);
            return Ok(page.ToResponse());
        }
    }
}
