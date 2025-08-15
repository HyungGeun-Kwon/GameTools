using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameTools.Domain.Common
{
    public abstract class Entity<TKey>
    {
        public TKey Id { get; protected set; } = default!;
    }
}
