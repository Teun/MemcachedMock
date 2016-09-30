using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class CasGetSet
    {
        private const string KEY = "abc";

        [TestMethod]
        public void StoreAndGetWithCas_Works()
        {
            Guid code = Guid.NewGuid();
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, code);
            var found = client.GetWithCas(KEY);
            Assert.IsTrue(found.Cas > 0);
            Assert.AreEqual(code, found.Result);
        }
        [TestMethod]
        public void AfterUpdate_CasGetsHigher()
        {
            Guid code = Guid.NewGuid();
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, code);
            var found1 = client.GetWithCas(KEY);
            client.Store(StoreMode.Set, KEY, Guid.NewGuid());
            var found2 = client.GetWithCas(KEY);

            Assert.IsTrue(found2.Cas > found1.Cas);
        }

    }
}
