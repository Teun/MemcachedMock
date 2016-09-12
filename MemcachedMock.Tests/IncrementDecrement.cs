using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class IncrementDecrement
    {
        private const string KEY = "abc";
        [TestMethod]
        public void AddThenIncrement()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23);
            ulong result = client.Increment(KEY, 1, 1);
            Assert.AreEqual(result, 24ul);
        }
        // increment non-numeric
        [TestMethod]
        public void IncrementNonNumeric_Throws()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, "abc");
            try
            {
                ulong result = client.Increment(KEY, 1, 1);
            }
            catch(Exception ex)
            {
                return;
            }

            Assert.Fail("Should throw an exception");
        }
        [TestMethod]
        public void IncrementWhenEmpty()
        {
            IMemcachedClient client = new CacheMock();
            ulong result = client.Increment(KEY, 1, 1);
            Assert.AreEqual(result, 1ul);
        }
        // decrement
        // decrement when empty

    }
}
