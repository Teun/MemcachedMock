using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class BasicStoreGet
    {
        private const string KEY = "abc";
        [TestMethod]
        public void StoreIntSucceeds()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23);
        }
        [TestMethod]
        public void StoreIntWithExpiresSucceeds()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23, DateTime.Today.AddDays(1));
        }
        [TestMethod]
        public void StoreIntWithTimespanSucceeds()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23, TimeSpan.FromMinutes(1));
        }
        [TestMethod]
        public void StoreIntAndGetGeneric_ReturnsSame()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23);
            int found = client.Get<int>(KEY);
            Assert.AreEqual(23, found);
        }
        [TestMethod]
        public void StoreIntAndGet_ReturnsSame()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23);
            object found = client.Get(KEY);
            Assert.AreEqual(23, found);
        }

        [TestMethod]
        public void StoreGuidAndGet_ReturnsSame()
        {
            Guid code = Guid.NewGuid();
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, code);
            object found = client.Get(KEY);
            Assert.AreEqual(code, found);
        }

    }
}
