# MemcachedMock
A fully functioning in-process mock of the Enyim memcached client. This mock is intended to be used in unit tests and tries to mimic the behavior of the enyim client backed by memcached.

It is NOT thread safe and NOT optimized for high load or large objects. Design is done with the purpose of testing only.

To test the behavior of memcached over time, you can have time elapse through the ICacheMeta iterface of the mock.