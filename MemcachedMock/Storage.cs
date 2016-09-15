using System;
using System.Linq;
using System.Collections.Generic;
using Enyim.Caching.Memcached;

namespace MemcachedMock
{
    internal class Storage
    {
        private static readonly ArraySegment<byte> NullArray = new ArraySegment<byte>(new byte[0]);
        private Dictionary<string, Record> _data = new Dictionary<string, Record>();

        private class Record
        {
            public DateTime Expires { get; set; }
            public CacheItem Data { get; set; }
        }
        public CacheItem Get(string key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key].Data;
            }
            else
            {
                return new CacheItem(DefaultTranscoder.TypeCodeToFlag(TypeCode.DBNull), NullArray);
            }
        }
        public void Set(string key, DateTime? expires, CacheItem data)
        {
            if(!expires.HasValue)
            {
                if (_data.ContainsKey(key))
                {
                    expires = _data[key].Expires;
                }
                else
                {
                    expires = DateTime.MaxValue;
                }
            }
            _data[key] = new Record() { Expires = expires.Value, Data = data };
        }
        public void Purge()
        {
            _data.Clear();
        }
        public void PurgeOnTime(DateTime now)
        {
            foreach (var key in _data.Keys.ToList())
            {
                if(_data[key].Expires < now)
                {
                    _data.Remove(key);
                }
            }
        }

        public bool Clear(string key)
        {
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}