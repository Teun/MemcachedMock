using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class Stats
    {
        private const string KEY = "abc";
        [TestMethod]
        public void SimpleGet_Uses1()
        {
            var client = new CacheMock();
            IStats stats = client.Statistics;
            object found = client.Get(KEY);
            Assert.AreEqual(null, found);
            Assert.AreEqual(1, stats.TotalRoundtrips);
        }
        [TestMethod]
        public void StoreThenRead_Uses2()
        {
            var client = new CacheMock();
            IStats stats = client.Statistics;
            client.Store(StoreMode.Add, KEY, "bla");
            object found = client.Get(KEY);
            Assert.AreEqual(2, stats.TotalRoundtrips);
            Assert.AreEqual(4, stats.EstimatedPacketCount);
        }
        [TestMethod]
        public void Increment_Uses1()
        {
            var client = new CacheMock();
            IStats stats = client.Statistics;
            client.Increment(KEY, 5, 1);
            Assert.AreEqual(1, stats.TotalRoundtrips);
            client.Increment(KEY, 5, 1);
            Assert.AreEqual(2, stats.TotalRoundtrips);
            client.Decrement(KEY, 5, 1);
            Assert.AreEqual(3, stats.TotalRoundtrips);
        }
        [TestMethod]
        public void SetAndGetLargeItem_UsesMorePackets()
        {
            var client = new CacheMock();
            IStats stats = client.Statistics;
            client.Store(StoreMode.Add, KEY, new int[2000]);
            object found = client.Get(KEY);
            Assert.AreEqual(2, stats.TotalRoundtrips);
            Assert.AreEqual(14, stats.EstimatedPacketCount);
        }


    }
}
