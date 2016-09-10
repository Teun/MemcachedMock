using System;
using System.Collections.Generic;

namespace MemcachedMock
{
    internal class Storage
    {
        private Dictionary<string, Record> _data = new Dictionary<string, Record>();

        private class Record
        {
            public DateTime? Expires { get; set; }
            public byte[] Data { get; set; }
        }
        public byte[] Get(string key)
        {
            if (_data.ContainsKey(key))
            {
                return _data[key].Data;
            }
            else
            {
                return null;
            }
        }
        public void Set(string key, DateTime? expires, byte[] data)
        {
            _data[key] = new Record() { Expires = expires, Data = data };
        }
        public void Purge()
        {
            _data.Clear();
        }
        public void PurgeOnTime(DateTime now)
        {
            foreach (var key in _data.Keys)
            {
                if(_data[key].Expires < now)
                {
                    _data.Remove(key);
                }
            }
        }
    }
}