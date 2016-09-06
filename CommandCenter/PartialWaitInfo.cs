using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter
{
    public class PartialWaitInfo : IComparable<PartialWaitInfo>
    {
        public DateTime time;
        public int capacity;
        public Resources production;
        public PartialWaitInfo(DateTime time)
        {
            this.time = time;
        }
        public PartialWaitInfo(DateTime time, int capacity, Resources production)
        {
            this.time = time;
            this.capacity = capacity;
            this.production = production;
        }
        int IComparable<PartialWaitInfo>.CompareTo(PartialWaitInfo other)
        {
            return time.CompareTo(other.time);
        }
    }
}
