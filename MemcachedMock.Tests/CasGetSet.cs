using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class CasGetSet : BaseCacheTest
    {
        private const string KEY = "abc";

        [TestMethod]
        public void StoreAndGetWithCas_Works()
        {
            Guid code = Guid.NewGuid();
            _client.Store(StoreMode.Set, KEY, code);
            var found = _client.GetWithCas(KEY);
            Assert.IsTrue(found.Cas > 0);
            Assert.AreEqual(code, found.Result);
        }
        [TestMethod]
        public void AfterUpdate_CasGetsHigher()
        {
            Guid code = Guid.NewGuid();
            _client.Store(StoreMode.Set, KEY, code);
            var found1 = _client.GetWithCas(KEY);
            _client.Store(StoreMode.Set, KEY, Guid.NewGuid());
            var found2 = _client.GetWithCas(KEY);

            Assert.IsTrue(found2.Cas > found1.Cas);
        }
        [TestMethod]
        public void AfterAppend_CasGetsHigher()
        {
            Guid code = Guid.NewGuid();
            _client.Store(StoreMode.Set, KEY, code);
            var found1 = _client.GetWithCas(KEY);
            _client.Append(KEY, new ArraySegment<byte>( Guid.NewGuid().ToByteArray()));
            var found2 = _client.GetWithCas(KEY);

            Assert.AreNotEqual(found2.Cas, found1.Cas);
        }
        [TestMethod]
        public void AfterPrepend_CasGetsHigher()
        {
            Guid code = Guid.NewGuid();
            _client.Store(StoreMode.Set, KEY, code.ToByteArray());
            var found1 = _client.GetWithCas(KEY);
            _client.Prepend(KEY, new ArraySegment<byte>(Guid.NewGuid().ToByteArray()));
            var found2 = _client.GetWithCas(KEY);

            Assert.AreNotEqual(found2.Cas, found1.Cas);
        }
        [TestMethod]
        public void IncrementWithCas_FailsWhenWrongExpectation()
        {
            _client.Store(StoreMode.Set, KEY, 4);
            var found = _client.GetWithCas(KEY);
            var casResult = _client.Increment(KEY, 1, 1, found.Cas-1);

            Assert.AreEqual(2, casResult.StatusCode);
        }
        [TestMethod]
        public void IncrementWithCas_SucceedsWhenCorrectExpectation()
        {
            _client.Store(StoreMode.Set, KEY, "4");
            var found = _client.GetWithCas(KEY);
            var casResult = _client.Increment(KEY, 1, 7, found.Cas);

            Assert.AreEqual(0, casResult.StatusCode);
            Assert.AreEqual(11UL, casResult.Result);
        }
        [TestMethod]
        public void DecrementWithCas_FailsWhenWrongExpectation()
        {
            IMemcachedClient client = new CacheMock();
            _client.Store(StoreMode.Set, KEY, 4);
            var found = _client.GetWithCas(KEY);
            var casResult = _client.Increment(KEY, 1, 1, found.Cas - 1);

            Assert.AreEqual(2, casResult.StatusCode);
        }
        [TestMethod]
        public void DecrementWithCas_SucceedsWhenCorrectExpectation()
        {
            _client.Store(StoreMode.Set, KEY, "4");
            var found = _client.GetWithCas(KEY);
            var casResult = _client.Decrement(KEY, 1, 2, found.Cas);

            Assert.AreEqual(0, casResult.StatusCode);
            Assert.AreEqual(2UL, casResult.Result);
        }

        [TestMethod]
        public void SetWithCas_FailsWhenWrongExpectation()
        {
            _client.Store(StoreMode.Set, KEY, 4);
            var found = _client.GetWithCas(KEY);
            var casResult = _client.Cas(StoreMode.Set, KEY, 3, found.Cas - 1);

            Assert.AreEqual(2, casResult.StatusCode);
            Assert.IsFalse(casResult.Result);
        }
        [TestMethod]
        public void SetWithCas_SucceedsWhenCorrectExpectation()
        {
            _client.Store(StoreMode.Set, KEY, 4);
            var found = _client.GetWithCas(KEY);
            var casResult = _client.Cas(StoreMode.Set, KEY, 3, found.Cas);

            Assert.AreEqual(0, casResult.StatusCode);
            Assert.IsTrue(casResult.Result);
        }


    }
}
