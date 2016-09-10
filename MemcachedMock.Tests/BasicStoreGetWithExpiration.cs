using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class BasicStoreGetWithExpiration
    {
        private const string KEY = "abc";
        [TestMethod]
        public void StoreIntAndGet_ReturnsSame_UntilTimeElapsed()
        {
            IMemcachedClient client = new CacheMock();
            ICacheMeta meta = client as ICacheMeta;
            meta.Time.Set(new DateTime(2016, 1, 1, 12, 23, 00));
            client.Store(StoreMode.Set, KEY, 23, TimeSpan.FromMinutes(20));
            meta.Time.Proceed(TimeSpan.FromMinutes(15));
            object found = client.Get(KEY);
            Assert.AreEqual(23, found);
            meta.Time.Proceed(TimeSpan.FromMinutes(6));
            found = client.Get(KEY);
            Assert.AreEqual(null, found);
        }

        [TestMethod]
        public void StoreIntAndGet_ReturnsSame_UntilTimeExpired()
        {
            IMemcachedClient client = new CacheMock();
            ICacheMeta meta = client as ICacheMeta;
            meta.Time.Set(new DateTime(2016, 1, 1, 12, 23, 00));
            client.Store(StoreMode.Set, KEY, 23, TimeSpan.FromMinutes(20));
            meta.Time.Set(new DateTime(2016, 1, 1, 12, 42, 00));
            object found = client.Get(KEY);
            Assert.AreEqual(23, found);
            meta.Time.Set(new DateTime(2016, 1, 1, 12, 44, 00));
            found = client.Get(KEY);
            Assert.AreEqual(null, found);
        }

    }
}
