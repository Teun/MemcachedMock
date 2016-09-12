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

        public ITime Time
        {
            get { return _time; } 
        }

        private void CheckUpToDate()
        {
            if(_store == null)
            {
                _store = new Storage();
            }
            _store.PurgeOnTime(_time.Now());
        }

        private byte[] AsBytes(object value)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                return stream.GetBuffer().Take((int)stream.Position).ToArray();
            }
        }

        private object FromBytes(byte[] data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                object result = formatter.Deserialize(stream);
                return result;
            }
        }

        private T FromBytes<T>(byte[] data)
        {
            return (T)FromBytes(data);
        }

        public event Action<IMemcachedNode> NodeFailed;

        public bool Append(string key, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }

        public CasResult<bool> Append(string key, ulong cas, ArraySegment<byte> data)
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
            throw new NotImplementedException();
        }

        public CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            throw new NotImplementedException();
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            throw new NotImplementedException();
        }

        public ulong Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            CheckUpToDate();
            byte[] data = _store.Get(key);
            if (data == null) return null;
            return FromBytes(data);
        }

        public T Get<T>(string key)
        {
            CheckUpToDate();
            byte[] data = _store.Get(key);
            if (data == null) return default(T);
            return FromBytes<T>(data);
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
            return PerformIncrement(key, defaultValue, delta, DateTime.MaxValue);
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor)
        {
            throw new NotImplementedException();
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, ulong cas)
        {
            throw new NotImplementedException();
        }

        public ulong Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            throw new NotImplementedException();
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas)
        {
            throw new NotImplementedException();
        }

        public CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas)
        {
            throw new NotImplementedException();
        }
        private ulong PerformIncrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt)
        {
            CheckUpToDate();
            object currValue = Get(key);
            if(currValue == null)
            {
                Store(StoreMode.Set, key, defaultValue, expiresAt);
                return defaultValue;
            }
            else
            {
                ulong currNumeric = Convert.ToUInt64(currValue);
                Store(StoreMode.Set, key, currNumeric+1, expiresAt);
                return currNumeric+1;
            }
        }


        public bool Prepend(string key, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }

        public CasResult<bool> Prepend(string key, ulong cas, ArraySegment<byte> data)
        {
            throw new NotImplementedException();
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
            DateTime end = _time.Now().Add(validFor);
            if (validFor == TimeSpan.Zero) end = DateTime.MaxValue;
            return PerformStore(mode, key, value, end);
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
                    current = this.Get(key);
                    if(current == null)
                    {
                        _store.Set(key, expiresAt, AsBytes(value));
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case StoreMode.Replace:
                    current = this.Get(key);
                    if (current == null)
                    {
                        return false;
                    }
                    else
                    {
                        _store.Set(key, expiresAt, AsBytes(value));
                        return true;
                    }
                default:
                    _store.Set(key, expiresAt, AsBytes(value));
                    return true;
            }
        }

        public bool TryGet(string key, out object value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetWithCas(string key, out CasResult<object> value)
        {
            throw new NotImplementedException();
        }
    }

    public interface ICacheMeta
    {
        ITime Time { get; }
    }
}
