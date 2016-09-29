using System;

namespace MemcachedMock
{
    public interface IStats
    {
        void Reset();
        int TotalRoundtrips { get; }
        int EstimatedPacketCount { get; }

    }
    public class StatsCounter : IStats
    {
        private int _calls = 0;
        private int _packets = 0;
        public StatsCounter()
        {
        }

        public void Increment(int calls, int packets)
        {
            _calls += calls;
            _packets += packets;
        }


        public int EstimatedPacketCount
        {
            get
            {
                return _packets;
            }
        }


        public int TotalRoundtrips
        {
            get
            {
                return _calls;
            }
        }

        public void Reset()
        {
            _calls = 0;
            _packets = 0;
        }
    }

}