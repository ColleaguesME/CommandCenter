using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter
{
    public class PartialFarmInfo : IComparable<PartialFarmInfo>
    {
        public DateTime time;
        public int limit;
        public PartialFarmInfo(DateTime time, int limit = 0)
        {
            this.time = time;
            this.limit = limit;
        }
        int IComparable<PartialFarmInfo>.CompareTo(PartialFarmInfo other)
        {
            return time.CompareTo(other.time);
        }
    }
}
