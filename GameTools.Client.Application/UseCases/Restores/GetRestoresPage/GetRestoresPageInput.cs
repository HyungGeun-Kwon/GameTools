using GameTools.Client.Application.Common.Paging;

namespace GameTools.Client.Application.UseCases.Restores.GetRestoresPage
{
    public sealed record GetRestoresPageInput(Pagination Pagination, RestoreSearchFilter? Filter);
}
