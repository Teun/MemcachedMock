using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemcachedMock
{
    static class CacheItemExtensions
    {
        private static uint emptyFlag = DefaultTranscoder.TypeCodeToFlag(TypeCode.DBNull);
        public static bool IsNull(this CacheItem me)
        {
            return me.Data.Count == 0 && me.Flags == emptyFlag;
        }
    }
}
