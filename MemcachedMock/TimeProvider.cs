using System;

namespace MemcachedMock
{
    internal class TimeProvider
    {
        private DateTime? FixedTime = null;
        internal DateTime Now()
        {
            if (FixedTime.HasValue)
            {
                return FixedTime.Value;
            }
            else
            {
                return DateTime.Now;
            }
        }
    }
}