using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class MultiGet
    {
        private const string KEY1 = "abc";
        private const string KEY2 = "abcd";

        [TestMethod]
        public void StoreGuidAndGet_ReturnsSame()
        {
            Guid code = Guid.NewGuid();
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY1, code, TimeSpan.FromMinutes(20));
            client.Store(StoreMode.Set, KEY2, code, TimeSpan.FromMinutes(30));

            var all = client.Get(new string[] { KEY1, KEY2 });
            Assert.AreEqual(2, all.Count);
            Assert.AreEqual(code, all[KEY1]);
            Assert.AreEqual(code, all[KEY2]);

            ((ICacheMeta)client).Time.Proceed(TimeSpan.FromMinutes(25));
            all = client.Get(new string[] { KEY1, KEY2 });
            Assert.AreEqual(2, all.Count);
            Assert.IsNull(all[KEY1]);
            Assert.IsNotNull(all[KEY2]);
        }

    }
}
