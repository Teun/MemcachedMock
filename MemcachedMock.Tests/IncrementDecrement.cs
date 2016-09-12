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
        [TestMethod]
        public void AddThenIncrement_WithTimespan()
        {
            IMemcachedClient client = new CacheMock();
            ICacheMeta meta = client as ICacheMeta;
            client.Store(StoreMode.Set, KEY, 23);
            client.Increment(KEY, 1, 1, TimeSpan.FromMinutes(15));
            meta.Time.Proceed(TimeSpan.FromMinutes(10));
            Assert.AreEqual(24ul, client.Get(KEY));
            meta.Time.Proceed(TimeSpan.FromMinutes(10));
            Assert.IsNull(client.Get(KEY));
        }
        [TestMethod]
        public void AddThenIncrement_WithExpires()
        {
            IMemcachedClient client = new CacheMock();
            ICacheMeta meta = client as ICacheMeta;
            client.Store(StoreMode.Set, KEY, 23);
            meta.Time.Set(new DateTime(2013, 1, 1, 8, 23, 00));
            client.Increment(KEY, 1, 1, new DateTime(2013, 1, 1, 8, 25, 00));
            meta.Time.Set(new DateTime(2013, 1, 1, 8, 24, 00));
            Assert.AreEqual(24ul, client.Get(KEY));
            meta.Time.Set(new DateTime(2013, 1, 1, 8, 25, 01));
            Assert.IsNull(client.Get(KEY));
        }
        // decrement
        [TestMethod]
        public void AddThenDecrement()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY, 23);
            ulong result = client.Decrement(KEY, 1, 1);
            Assert.AreEqual(result, 22ul);
        }
        [TestMethod]
        public void DecrementWhenEmpty()
        {
            IMemcachedClient client = new CacheMock();
            ulong result = client.Decrement(KEY, 1, 1);
            Assert.AreEqual(result, 1ul);
        }

        // decrement when empty

    }
}
