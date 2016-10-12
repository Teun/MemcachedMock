using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MemcachedMock.Tests
{
    public class BaseCacheTest
    {
        protected IMemcachedClient _client = null;
        protected ICacheMeta _meta;

        [TestInitialize]
        public void Init()
        {
            if (true)
            {
                _client = new CacheMock();
                _meta = (ICacheMeta)_client;
            }
            else
            {
                var cfg = new MemcachedClientConfiguration();
                cfg.AddServer("127.0.0.1", 11211); cfg.Protocol = MemcachedProtocol.Binary;
                _client = new MemcachedClient(cfg);
                _client.FlushAll();
            }
        }
    }
}