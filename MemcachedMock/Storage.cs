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
        private ulong _ticker = 0;

        public Storage()
        {
            _ticker = Convert.ToUInt64(DateTime.Now.Ticks);
        }

        private void IncTicker()
        {
            _ticker++;
        }

        private class Record
        {
            public DateTime Expires { get; set; }
            public CacheItem Data { get; set; }
            public ulong Tick { get; set; }
        }
        public CacheItem Get(string key) {
            ulong dontCare;
            return Get(key, out dontCare);
        }
        public CacheItem Get(string key, out ulong casValue)
        {
            if (_data.ContainsKey(key))
            {
                casValue = _data[key].Tick;
                return _data[key].Data;
            }
            else
            {
                casValue = 0;
                return new CacheItem(DefaultTranscoder.TypeCodeToFlag(TypeCode.DBNull), NullArray);
            }
        }
        public ulong Set(string key, DateTime? expires, CacheItem data)
        {
            IncTicker();
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
            _data[key] = new Record() { Expires = expires.Value, Data = data , Tick=_ticker};
            return _ticker;
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