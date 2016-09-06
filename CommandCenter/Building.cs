using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CommandCenter
{
    [DataContract]
    public abstract class Building : Entity
    {
        protected static void ReadFromFile(string fileName, ref Resources[] resources, ref int[] provisions, ref int[] significance, ref int[] time, ref int requirements, ref int maxLevel)
        {
            string[] file = Properties.Resources.ResourceManager.GetString(fileName.ToLower()).Split('\n');
            string[] info = file[0].Split(' ');
            maxLevel = int.Parse(info[0]);
            int woodColumn = 0,
                clayColumn = 1,
                ironColumn = 2,
                provColumn = 3,
                timeColumn = 4,
                significanceColumn = 5;
            requirements = int.Parse(info[1]);
            resources = new Resources[maxLevel + 1];
            provisions = new int[maxLevel + 1];
            time = new int[maxLevel + 1];
            significance = new int[maxLevel + 1];
            for (int Level = 0; Level <= maxLevel; Level++)
            {
                info = file[Level + 1].Split('\t');
                resources[Level] = new Resources(int.Parse(info[woodColumn]), int.Parse(info[clayColumn]), int.Parse(info[ironColumn]));
                provisions[Level] = int.Parse(info[provColumn]);
                string[] date = info[timeColumn].Split(':');
                time[Level] = int.Parse(date[0]) * 3600 + int.Parse(date[1]) * 60 + int.Parse(date[2]);
                significance[Level] = int.Parse(info[significanceColumn]);
            }
        }
        [DataMember]
        public int Level;
        
        public void Update(Building toUpdate)
        {
            Level = toUpdate.Level;
        }
        public override void Up(int number = 1)
        {
            Level += number;
        }
    }
}
