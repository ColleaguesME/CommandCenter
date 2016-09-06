using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Units
{
    public class Spearman : Unit
    {
        protected static Resources resources;
        protected static int provisions, time, requirements;
        static Spearman()
        {
            ReadFromFile(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, ref resources, ref provisions, ref time, ref requirements);
        }
        public override Resources GetResources()
        {
            return resources;
        }
        public override int GetProvisions()
        {
            return provisions;
        }
        public override int GetTime()
        {
            return time;
        }
        public override int GetRequirements()
        {
            return requirements;
        }
    }
}
