using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class StoreReplace
    {
        private const string KEY = "abc";
        [TestMethod]
        public void ReplaceIntFailsWhenEmpty()
        {
            IMemcachedClient client = new CacheMock();
            bool result = client.Store(StoreMode.Replace, KEY, 23);
            Assert.IsFalse(result);
        }
        [TestMethod]
        public void AddThenReplaceIntSucceeds()
        {
            IMemcachedClient client = new CacheMock();
            bool result = client.Store(StoreMode.Add, KEY, 23);
            result = client.Store(StoreMode.Replace, KEY, 24);
            Assert.IsTrue(result);

            int stored = client.Get<int>(KEY);
            Assert.AreEqual(24, stored);
        }

    }
}
