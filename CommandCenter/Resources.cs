using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter
{
    [DataContract]
    public class Resources
    {
        public const double DeltaTime = 3600.0;
        [DataMember]
        int wood, clay, iron;
        public Resources(int wood, int clay, int iron)
        {
            this.wood = wood;
            this.clay = clay;
            this.iron = iron;
        }
        public int GetWaitingTime(Resources Production)
        {
            double time = 0;
            time = Math.Max(time, wood * DeltaTime / Production.wood);
            time = Math.Max(time, clay * DeltaTime / Production.clay);
            time = Math.Max(time, iron * DeltaTime / Production.iron);
            return Convert.ToInt32(Math.Ceiling(time));
        }
        public void Ceil(int capacity)
        {
            if (wood > capacity)
            {
                wood = capacity;
            }
            if (clay > capacity)
            {
                clay = capacity;
            }
            if (iron > capacity)
            {
                iron = capacity;
            }
        }
        public void Floor()
        {
            if (wood < 0)
            {
                wood = 0;
            }
            if (clay < 0)
            {
                clay = 0;
            }
            if (iron < 0)
            {
                iron = 0;
            }
        }

        public static Resources operator +(Resources left, Resources right)
        {
            return new Resources(left.wood + right.wood, left.clay + right.clay, left.iron + right.iron);
        }
        public static Resources operator -(Resources left, Resources right)
        {
            return new Resources(left.wood - right.wood, left.clay - right.clay, left.iron - right.iron);
        }
        public static Resources operator *(Resources left, int right)
        {
            return new Resources(left.wood * right, left.clay * right, left.iron * right);
        }
        public static Resources operator *(Resources left, double right)
        {
            return new Resources(Convert.ToInt32(left.wood * right), Convert.ToInt32(left.clay * right), Convert.ToInt32(left.iron * right));
        }

        public static bool operator <(Resources left, int right)
        {
            return left.wood < right && left.clay < right && left.iron < right;
        }
        public static bool operator >(Resources left, int right)
        {
            return left.wood > right || left.clay > right || left.iron > right;
        }
        public static bool operator <=(Resources left, int right)
        {
            return left.wood <= right && left.clay <= right && left.iron <= right;
        }
        public static bool operator >=(Resources left, int right)
        {
            return left.wood >= right || left.clay >= right || left.iron >= right;
        }
        public int Wood
        {
            get
            {
                return wood;
            }
        }
        public int Clay
        {
            get
            {
                return clay;
            }
        }
        public int Iron
        {
            get
            {
                return iron;
            }
        }
    }
}
