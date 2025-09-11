using System.Net.Mime;
using GameTools.Contracts.Common;
using GameTools.Contracts.Items.GetItemAuditPage;
using GameTools.Server.Api.Mapper;
using GameTools.Server.Application.Features.Audit.Queries.GetItemAuditPage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameTools.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Tags("Audits")]
    public sealed class AuditsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("items-page-search")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<PagedResponse<ItemAuditReadModel>>> GetItemAudits(
            [FromBody] ItemAuditsPageRequest request, CancellationToken ct = default)
        {
            var criteria = request.ToCriteria();
            var query = new GetItemAuditPageQuery(criteria);
            var page = await mediator.Send(query, ct);
            return Ok(page.ToResponse());
        }
    }
}
