using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Configuration;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class Reference
    {
        [TestMethod, Ignore]
        public void TestMethod1()
        {
            string KEY = "1234rewqq";
            var cfg = new MemcachedClientConfiguration();
            cfg.AddServer("127.0.0.1", 11211); cfg.Protocol = MemcachedProtocol.Binary; 
            IMemcachedClient client = new MemcachedClient(cfg);
            client.FlushAll();
            client.Store(StoreMode.Set, KEY, 4);
            var found = client.GetWithCas(KEY);
            var casResult = client.Increment(KEY, 1, 1, found.Cas - 1);

            Assert.AreEqual(2, casResult.StatusCode);
            Assert.AreEqual(0, casResult.Result);
        }
    }
}
