using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Server.Application.Abstractions.UnitOfWorks
{
    public interface IUnitOfWorkTransaction : IAsyncDisposable
    {
        Task CommitAsync(CancellationToken ct = default);
        Task RollbackAsync(CancellationToken ct = default);
    }
}
