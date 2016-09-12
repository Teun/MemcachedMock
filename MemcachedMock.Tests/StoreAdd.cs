using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class StoreAdd
    {
        private const string KEY = "abc";
        [TestMethod]
        public void AddIntSucceedsWhenEmpty()
        {
            IMemcachedClient client = new CacheMock();
            bool result = client.Store(StoreMode.Add, KEY, 23);
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void AddIntTwiceFails()
        {
            IMemcachedClient client = new CacheMock();
            bool result = client.Store(StoreMode.Add, KEY, 23);
            result = client.Store(StoreMode.Add, KEY, 24);
            Assert.IsFalse(result);

            int stored = client.Get<int>(KEY);
            Assert.AreEqual(23, stored);
        }

    }
}
