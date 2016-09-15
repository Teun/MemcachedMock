using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace MemcachedMock.Tests
{
    [TestClass]
    public class AppendPrepend
    {
        private const string KEY1 = "abc";

        [TestMethod]
        public void StoreByteArrayThenAppend()
        {
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY1, bytes);

            client.Append(KEY1, new ArraySegment<byte>(new byte[] { 8, 9 }));

            var stored = client.Get<byte[]>(KEY1);
            Assert.AreEqual(9, stored.Length);
        }

        [TestMethod]
        public void AppendWhenEmpty_Fails()
        {
            IMemcachedClient client = new CacheMock();

            var result = client.Append(KEY1, new ArraySegment<byte>(new byte[] { 8, 9 }));

            Assert.IsFalse(result);
        }
        [TestMethod]
        public void StoreIntThenAppendBytes()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY1, 17);

            var result = client.Append(KEY1, new ArraySegment<byte>(new byte[] { 8, 9 }));
            Assert.IsTrue(result);

            var stored = client.Get(KEY1);

            // this may seem strange, but this is what Enyim client does: first we store an int as 4 bytes,
            // with the flags to indicate that it is an int. Then we append 2 more bytes (succesfully), 
            // but the flags still indicate Int32, so when deserializing, the last two bytes are ignored.
            Assert.AreEqual(17, stored);
        }
        [TestMethod]
        public void StoreStringThenAppendBytes()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY1, "May");

            var result = client.Append(KEY1, new ArraySegment<byte>(new byte[] { (byte)'a' }));
            Assert.IsTrue(result);

            var stored = client.Get(KEY1);

            Assert.AreEqual("Maya", stored);
        }
        [TestMethod]
        public void StoreByteArrayThenPrepend()
        {
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7 };
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY1, bytes);

            client.Prepend(KEY1, new ArraySegment<byte>(new byte[] { 8, 9 }));

            var stored = client.Get<byte[]>(KEY1);
            Assert.AreEqual(9, stored.Length);
            Assert.AreEqual(9, stored[1]);
        }

        [TestMethod]
        public void PrependWhenEmpty_Fails()
        {
            IMemcachedClient client = new CacheMock();

            var result = client.Prepend(KEY1, new ArraySegment<byte>(new byte[] { 8, 9 }));

            Assert.IsFalse(result);
        }
        [TestMethod]
        public void StoreIntThenPrependBytes()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY1, 17);

            var result = client.Prepend(KEY1, new ArraySegment<byte>(new byte[] { 8, 9 }));
            Assert.IsTrue(result);

            var stored = client.Get(KEY1);

            // this may seem strange, but this is what Enyim client does: first we store an int as 4 bytes,
            // with the flags to indicate that it is an int. Then we prepend 2 more bytes (succesfully), 
            // but the flags still indicate Int32, so when deserializing we get the prepended bytes + the first
            // 2 bytes of the original int32 together interpreted as an Int32.
            Assert.AreEqual(1116424, stored);
        }
        [TestMethod]
        public void StoreStringThenPrependBytes()
        {
            IMemcachedClient client = new CacheMock();
            client.Store(StoreMode.Set, KEY1, "May");

            var result = client.Prepend(KEY1, new ArraySegment<byte>(new byte[] { (byte)'a' }));
            Assert.IsTrue(result);

            var stored = client.Get(KEY1);

            Assert.AreEqual("aMay", stored);
        }

    }
}
