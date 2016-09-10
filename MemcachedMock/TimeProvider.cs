using System;

namespace MemcachedMock
{
    public interface ITime
    {
        void Set(DateTime value);
        void Proceed(TimeSpan time);
        DateTime Now();
    }
    internal class TimeProvider : ITime
    {
        private DateTime? _fixedTime = null;

        public void Proceed(TimeSpan time)
        {
            if (!_fixedTime.HasValue) _fixedTime = DateTime.Now;
            _fixedTime = _fixedTime.Value.Add(time);
        }

        public void Set(DateTime value)
        {
            _fixedTime = value;
        }

        public DateTime Now()
        {
            if (_fixedTime.HasValue)
            {
                return _fixedTime.Value;
            }
            else
            {
                return DateTime.Now;
            }
        }
    }
}