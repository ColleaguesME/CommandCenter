using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter
{
    public class PartialBarracksInfo : IComparable<PartialBarracksInfo>
    {
        public DateTime time;
        public double significance;
        public PartialBarracksInfo(DateTime time, double significance = 0)
        {
            this.time = time;
            this.significance = significance;
        }
        int IComparable<PartialBarracksInfo>.CompareTo(PartialBarracksInfo other)
        {
            return time.CompareTo(other.time);
        }
    }
}
