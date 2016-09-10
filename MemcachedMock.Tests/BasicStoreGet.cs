using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class BasicStoreGet
    {
        [TestMethod]
        public void StoreIntSucceeds()
        {
            IMemcachedClient client = new CacheMock();

            client.Store(StoreMode.Set, "abc", 23);
        }
        [TestMethod]
        public void StoreIntWithExpiresSucceeds()
        {
            IMemcachedClient client = new CacheMock();

            client.Store(StoreMode.Set, "abc", 23, DateTime.Today.AddDays(1));
        }
        [TestMethod]
        public void StoreIntWithTimespanSucceeds()
        {
            IMemcachedClient client = new CacheMock();

            client.Store(StoreMode.Set, "abc", 23, TimeSpan.FromMinutes(1));
        }
    }
}
