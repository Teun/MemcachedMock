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
            _stats.Increment(1, (int)Math.Ceiling(((double)data.Count / 1400)));
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
            return PerformStore(mode, key, value, DateTime.MaxValue, 0);
        }

        public CasResult<bool> Cas(StoreMode mode, string key, object value, ulong cas)
        {
            return PerformStore(mode, key, value, DateTime.MaxValue, cas);
        }

        public CasResult<bool> Cas(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas)
        {
            return PerformStore(mode, key, value, EndTime(validFor), cas);
        }

        public CasResult<bool> Cas(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas)
        {
            return PerformStore(mode, key, value, expiresAt, cas);
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta)
        {
            return PerformIncrement(key, defaultValue, -Convert.ToInt64(delta), DateTime.MaxValue);
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            return PerformIncrement(key, defaultValue, -(long)delta, DateTime.MaxValue, ref cas);
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            return PerformIncrement(key, defaultValue, -Convert.ToInt64(delta), EndTime(validFor));
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            return PerformIncrement(key, defaultValue, -Convert.ToInt64(delta), expiresAt);
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
            return keys.ToDictionary(s => s, s => PerformGet(s, false));
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
        private object PerformGet(string key, bool count = true)
        {
            ulong dontcare;
            return PerformGet(key, out dontcare, count);
        }
        private object PerformGet(string key, out ulong cas, bool count = true)
        {
            CheckUpToDate();
            CacheItem data = _store.Get(key, out cas);
            if(count) _stats.Increment(1, 1 + data.PacketSize());
            if (data.Data.Count == 0) return null;
            return FromCacheItem(data);
        }

        public IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys)
        {
            return keys.Select(k => new { Key = k, Val = GetWithCas(k) }).ToDictionary(item => item.Key, item => item.Val);
        }

        public CasResult<object> GetWithCas(string key)
        {
            ulong cas;
            object result = PerformGet(key, out cas);
            return new CasResult<object>() { Result = result, Cas = cas, StatusCode = 0 };
        }

        public CasResult<T> GetWithCas<T>(string key)
        {
            ulong cas;
            T result = (T)PerformGet(key, out cas);
            return new CasResult<T>() { Result = result, Cas = cas, StatusCode = 0 };
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta)
        {
            return PerformIncrement(key, defaultValue, Convert.ToInt64(delta), DateTime.MaxValue);
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            return PerformIncrement(key, defaultValue, Convert.ToInt64(delta), EndTime(validFor));
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            return PerformIncrement(key, defaultValue, Convert.ToInt64(delta), DateTime.MaxValue, ref cas);
            return default(CasResult<ulong>);
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            return PerformIncrement(key, defaultValue, Convert.ToInt64(delta), expiresAt);
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas)
        {
            return PerformIncrement(key, defaultValue, (long)delta, EndTime(validFor), ref cas);
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas)
        {
            throw new NotImplementedException();
        }
        private ulong PerformIncrement(string key, ulong defaultValue, long delta, DateTime expiresAt)
        {
            ulong dontCare = 0;
            var cr =  PerformIncrement(key, defaultValue, delta, expiresAt, ref dontCare);
            return cr.Result;
        }

        private CasResult<ulong> PerformIncrement(string key, ulong defaultValue, long delta, DateTime expiresAt, ref ulong cas)
        {
            CheckUpToDate();
            ulong oldCas;
            object currValue = PerformGet(key, out oldCas, false);
            if(currValue == null)
            {
                Store(StoreMode.Set, key, (ulong)defaultValue, expiresAt);
                return new CasResult<ulong>() { Result = (ulong)defaultValue, Cas = 0, StatusCode = 0 };
            }
            else
            {
                long currNumeric = Convert.ToInt64(currValue);
                if (cas > 0 && cas != oldCas)
                {
                    cas = oldCas;
                    return new CasResult<ulong>{ Result = (ulong)0, Cas = oldCas, StatusCode = 2 };
                }
                Store(StoreMode.Set, key, (ulong)(currNumeric+delta), expiresAt);
                ulong newCas;
                currValue = PerformGet(key, out newCas, false);
                return new CasResult<ulong>{ Result = (ulong)currValue, Cas = newCas, StatusCode = 0 };
            }
        }

        public bool Remove(string key)
        {
            _stats.Increment(1, 2);
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
            var res = PerformStore(mode, key, value, expiresAt, 0);
            return res.Result;
        }
        private CasResult<bool> PerformStore(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas)
        {
            CheckUpToDate();
            CacheItem ci = AsCacheItem(value);
            _stats.Increment(1, 1 + ci.PacketSize());
            ulong oldCas;
            object current = this.PerformGet(key, out oldCas, false); ;

            if(cas != 0 && cas != oldCas)
            {
                return new CasResult<bool> { StatusCode = 2 };
            }

            switch (mode)
            {
                case StoreMode.Add:
                    if(current == null)
                    {
                        ulong newCas = _store.Set(key, expiresAt, ci);
                        return new CasResult<bool> { Result = true, Cas = newCas, StatusCode = 0 };
                    }
                    else
                    {
                        return new CasResult<bool> { Result = false, Cas = 0, StatusCode = 0 };
                    }
                case StoreMode.Replace:
                    if (current == null)
                    {
                        return new CasResult<bool> { Result = false, Cas = 0, StatusCode = 0 };
                    }
                    else
                    {
                        ulong newCas = _store.Set(key, expiresAt, ci);
                        return new CasResult<bool> { Result = true, Cas = newCas, StatusCode = 0 };
                    }
                default:
                    {
                        ulong newCas = _store.Set(key, expiresAt, ci);
                        return new CasResult<bool> { Result = true, Cas = newCas, StatusCode = 0 };
                    }
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
