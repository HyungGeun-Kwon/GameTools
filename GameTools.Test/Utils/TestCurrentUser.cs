using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTools.Application.Abstractions.Users;

namespace GameTools.Test.Utils
{
    public class TestCurrentUser : ICurrentUser
    {
        public string UserIdOrName => "test_user";
    }
}
