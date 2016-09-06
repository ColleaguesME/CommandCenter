using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter.Buildings
{
    public class Academy : Building
    {
        protected static Resources[] resources;
        protected static int[] provisions, significance, time;
        protected static int requirements, maxLevel;
        static Academy()
        {
            ReadFromFile(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, ref resources, ref provisions, ref significance, ref time, ref requirements, ref maxLevel);
        }
        public override Resources GetResources()
        {
            return resources[Level + 1];
        }
        public override int GetProvisions()
        {
            return provisions[Level + 1];
        }
        public override int GetTime()
        {
            return time[Level + 1];
        }
        public override int GetRequirements()
        {
            return requirements;
        }
        public bool IsNotMax()
        {
            return Level != maxLevel;
        }
    }
}
