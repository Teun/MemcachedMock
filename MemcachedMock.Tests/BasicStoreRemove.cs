using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class BasicStoreRemove
    {
        private const string KEY = "abc";
        [TestMethod]
        public void StoreGuidAndRemove_Clears()
        {
            Guid code = Guid.NewGuid();
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, code);
            object found = client.Get(KEY);
            Assert.AreEqual(code, found);
            client.Remove(KEY);
            found = client.Get(KEY);
            Assert.AreEqual(null, found);
        }

    }
}
