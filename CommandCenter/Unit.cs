using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter
{
    [DataContract]
    public abstract class Unit : Entity
    {
        protected static void ReadFromFile(string fileName, ref Resources resources, ref int provisions, ref int time, ref int requirements)
        {
            string[] file = Properties.Resources.ResourceManager.GetString(fileName.ToLower()).Split('\n');
            requirements = int.Parse(file[0]);
            string[] info = file[1].Split('\t');
            resources = new Resources(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]));
            provisions = int.Parse(info[3]);
            DateTime date = DateTime.Parse(info[4]);
            time = date.Hour * 3600 + date.Minute * 60 + date.Second;
        }
        [DataMember]
        public int Quantity;
        public void Update(Unit toUpdate)
        {
            Quantity = toUpdate.Quantity;
        }
        public override void Up(int number)
        {
            Quantity += number;
        }
    }
}
