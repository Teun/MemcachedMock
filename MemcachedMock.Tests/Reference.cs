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
            var cfg = new MemcachedClientConfiguration();
            cfg.AddServer("127.0.0.1", 11211); cfg.Protocol = MemcachedProtocol.Text; 
            IMemcachedClient client = new MemcachedClient(cfg);
            client.FlushAll();
            client.Store(StoreMode.Set, "abc", "fvafvsfva");
            var appRes = client.Prepend("abc", new ArraySegment<byte>(new byte[] { 6, 7, 8 }));
            var result = client.Get("abc");
        }
    }
}
