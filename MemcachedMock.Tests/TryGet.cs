using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class TryGet
    {
        private const string KEY = "abc";
        [TestMethod]
        public void StoreIntAndTryGet_ReturnsTrue()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23);
            object val;
            bool found = client.TryGet(KEY, out val);
            Assert.IsTrue(found);
            Assert.AreEqual(val, 23);
        }
        [TestMethod]
        public void StoreIntAndTryGetAfterExpire_ReturnsFalse()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23, TimeSpan.FromHours(3));
            ((ICacheMeta)client).Time.Proceed(TimeSpan.FromMinutes(179));

            object val;
            bool found = client.TryGet(KEY, out val);
            Assert.IsTrue(found);
            Assert.AreEqual(val, 23);

            ((ICacheMeta)client).Time.Proceed(TimeSpan.FromMinutes(2));
            found = client.TryGet(KEY, out val);
            Assert.IsFalse(found);
            Assert.IsNull(val);
        }

    }
}
