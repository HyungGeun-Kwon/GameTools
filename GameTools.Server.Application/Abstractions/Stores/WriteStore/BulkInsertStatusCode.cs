using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTools.Server.Application.Abstractions.Stores.WriteStore
{
    public enum BulkInsertStatusCode : byte
    {
        /// <summary>삽입됨</summary>
        Inserted = 0,

        /// <summary>이름 중복 (배치 내 중복 또는 DB에 이미 존재)</summary>
        DuplicateName = 1,

        /// <summary>존재하지 않는 RarityId 참조</summary>
        InvalidRarity = 2,

        /// <summary>유효하지 않은 가격(음수 등)</summary>
        InvalidPrice = 3,
    }
}
