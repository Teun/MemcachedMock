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
    static class ArraySegmentExtensions
    {
        public static T[] AsArray<T>(this ArraySegment<T> me)
        {
            var result = new T[me.Count];
            Array.Copy(me.Array, me.Offset, result, 0, me.Count);
            return result;
        }
    }
}
