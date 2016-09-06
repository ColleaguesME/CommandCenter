using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommandCenter
{
    public abstract class Entity
    {
        public abstract Resources GetResources();
        public abstract int GetProvisions();
        public abstract int GetTime();
        public abstract int GetRequirements();
        public abstract void Up(int number);
    }
}
