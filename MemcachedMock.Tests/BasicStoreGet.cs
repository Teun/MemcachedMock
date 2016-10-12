using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class BasicStoreGet : BaseCacheTest
    {
        private const string KEY = "abc";
        [TestMethod]
        public void StoreIntSucceeds()
        {
            _client.Store(StoreMode.Set, KEY, 23);
        }
        [TestMethod]
        public void StoreIntWithExpiresSucceeds()
        {
            _client.Store(StoreMode.Set, KEY, 23, DateTime.Today.AddDays(1));
        }
        [TestMethod]
        public void StoreIntWithTimespanSucceeds()
        {
            _client.Store(StoreMode.Set, KEY, 23, TimeSpan.FromMinutes(1));
        }
        [TestMethod]
        public void StoreIntAndGetGeneric_ReturnsSame()
        {
            _client.Store(StoreMode.Set, KEY, 23);
            int found = _client.Get<int>(KEY);
            Assert.AreEqual(23, found);
        }
        [TestMethod]
        public void StoreIntAndGet_ReturnsSame()
        {
            _client.Store(StoreMode.Set, KEY, 23);
            object found = _client.Get(KEY);
            Assert.AreEqual(23, found);
        }

        [TestMethod]
        public void StoreGuidAndGet_ReturnsSame()
        {
            Guid code = Guid.NewGuid();
            _client.Store(StoreMode.Set, KEY, code);
            object found = _client.Get(KEY);
            Assert.AreEqual(code, found);
        }

    }
}
