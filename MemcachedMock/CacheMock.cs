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
            throw new NotImplementedException();
        }

        public void FlushAll()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            CheckUpToDate();
            _store.Set(key, null, AsBytes(value));
            return true;
        }

        public bool Store(StoreMode mode, string key, object value, TimeSpan validFor)
        {
            CheckUpToDate();
            _store.Set(key, _time.Now().Add(validFor), AsBytes(value));
            return true;
        }

        public bool Store(StoreMode mode, string key, object value, DateTime expiresAt)
        {
            CheckUpToDate();
            _store.Set(key, expiresAt, AsBytes(value));
            return true;
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
