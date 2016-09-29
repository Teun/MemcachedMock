using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Enyim.Caching;

namespace MemcachedMock
{
    public class CacheMock : IMemcachedClient, ICacheMeta
    {
        Storage _store;
        TimeProvider _time = new TimeProvider();
        ITranscoder _transcoder = new DefaultTranscoder();
        StatsCounter _stats = new StatsCounter();

        public ITime Time
        {
            get { return _time; } 
        }
        public IStats Statistics
        {
            get { return _stats; }
        }
        #region Helpers
        private void CheckUpToDate()
        {
            if(_store == null)
            {
                _store = new Storage();
            }
            _store.PurgeOnTime(_time.Now());
        }

        private CacheItem AsCacheItem(object value)
        {
            return _transcoder.Serialize(value);
        }

        private object FromCacheItem(CacheItem data)
        {
            return _transcoder.Deserialize(data);
        }

        private T FromCacheItem<T>(CacheItem data)
        {
            return (T)FromCacheItem(data);
        }
        private DateTime EndTime(TimeSpan ts)
        {
            DateTime end = _time.Now().Add(ts);
            if (ts == TimeSpan.Zero) end = DateTime.MaxValue;
            return end;
        }
        #endregion

        public event Action<IMemcachedNode> NodeFailed;

        public bool Append(string key, ArraySegment<byte> data)
        {
            return PerformCombine(key, data);
        }

        private bool PerformCombine(string key, ArraySegment<byte> data, bool atEnd = true)
        {
            CheckUpToDate();
            CacheItem current = _store.Get(key);
            if (current.IsNull())
            {
                return false;
            }
            byte[] newVal = (atEnd ? current.Data.AsArray().Concat(data.AsArray()) : data.AsArray().Concat(current.Data.AsArray())).ToArray();
            current.Data = new ArraySegment<byte>(newVal);
            //current.Flags = DefaultTranscoder.RawDataFlag;
            _store.Set(key, null, current);
            return true;
        }

        public CasResult<bool> Append(string key, ulong cas, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }

        public bool Prepend(string key, ArraySegment<byte> data)
        {
            return PerformCombine(key, data, false);
        }

        public CasResult<bool> Prepend(string key, ulong cas, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }


        public CasResult<bool> Cas(StoreMode mode, string key, object value)
        {
            throw new NotImplementedException();
        }

        public CasResult<bool> Cas(StoreMode mode, string key, object value, ulong cas)
        {
            throw new NotImplementedException();
        }

        public CasResult<bool> Cas(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas)
        {
            throw new NotImplementedException();
        }

        public CasResult<bool> Cas(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas)
        {
            throw new NotImplementedException();
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta)
        {
            return PerformIncrement(key, Convert.ToInt64(defaultValue), -Convert.ToInt64(delta), DateTime.MaxValue);
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            throw new NotImplementedException();
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            return PerformIncrement(key, Convert.ToInt64(defaultValue), -Convert.ToInt64(delta), EndTime(validFor));
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            return PerformIncrement(key, Convert.ToInt64(defaultValue), -Convert.ToInt64(delta), expiresAt);
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas)
        {
            throw new NotImplementedException();
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public void FlushAll()
        {
            _store = new Storage();
        }

        public IDictionary<string, object> Get(IEnumerable<string> keys)
        {
            return keys.ToDictionary(s => s, s => PerformGet(s));
        }

        public object Get(string key)
        {
            return PerformGet(key);
        }

        public T Get<T>(string key)
        {
            object val = PerformGet(key);
            if (val == null) return default(T);
            return (T)val;
        }
        private object PerformGet(string key)
        {
            CheckUpToDate();
            CacheItem data = _store.Get(key);
            if (data.Data.Count == 0) return null;
            return FromCacheItem(data);
        }

        public IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys)
        {
            throw new NotImplementedException();
        }

        public CasResult<object> GetWithCas(string key)
        {
            throw new NotImplementedException();
        }

        public CasResult<T> GetWithCas<T>(string key)
        {
            throw new NotImplementedException();
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta)
        {
            return PerformIncrement(key, Convert.ToInt64(defaultValue), Convert.ToInt64(delta), DateTime.MaxValue);
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            return PerformIncrement(key, Convert.ToInt64(defaultValue), Convert.ToInt64(delta), EndTime(validFor));
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            throw new NotImplementedException();
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            return PerformIncrement(key, Convert.ToInt64(defaultValue), Convert.ToInt64(delta), expiresAt);
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas)
        {
            throw new NotImplementedException();
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas)
        {
            throw new NotImplementedException();
        }
        private ulong PerformIncrement(string key, long defaultValue, long delta, DateTime expiresAt)
        {
            CheckUpToDate();
            object currValue = PerformGet(key);
            if(currValue == null)
            {
                Store(StoreMode.Set, key, (ulong)defaultValue, expiresAt);
                return (ulong)defaultValue;
            }
            else
            {
                long currNumeric = Convert.ToInt64(currValue);
                Store(StoreMode.Set, key, (ulong)(currNumeric+delta), expiresAt);
                return (ulong)(currNumeric+delta);
            }
        }

        public bool Remove(string key)
        {
            return _store.Clear(key);
        }

        public ServerStats Stats()
        {
            throw new NotImplementedException();
        }

        public ServerStats Stats(string type)
        {
            throw new NotImplementedException();
        }

        public bool Store(StoreMode mode, string key, object value)
        {
            return PerformStore(mode, key, value, DateTime.MaxValue);
        }

        public bool Store(StoreMode mode, string key, object value, TimeSpan validFor)
        {
            return PerformStore(mode, key, value, EndTime(validFor));
        }

        public bool Store(StoreMode mode, string key, object value, DateTime expiresAt)
        {
            return PerformStore(mode, key, value, expiresAt);
        }
        private bool PerformStore(StoreMode mode, string key, object value, DateTime expiresAt)
        {
            CheckUpToDate();
            object current;
            switch (mode)
            {
                case StoreMode.Add:
                    current = this.PerformGet(key);
                    if(current == null)
                    {
                        _store.Set(key, expiresAt, AsCacheItem(value));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case StoreMode.Replace:
                    current = this.PerformGet(key);
                    if (current == null)
                    {
                        return false;
                    }
                    else
                    {
                        _store.Set(key, expiresAt, AsCacheItem(value));
                        return true;
                    }
                default:
                    _store.Set(key, expiresAt, AsCacheItem(value));
                    return true;
            }
        }

        public bool TryGet(string key, out object value)
        {
            value = PerformGet(key);
            return value != null;
        }

        public bool TryGetWithCas(string key, out CasResult<object> value)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICacheMeta
    {
        ITime Time { get; }
        IStats Statistics { get; }
    }
}
