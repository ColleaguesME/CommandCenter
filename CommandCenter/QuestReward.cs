using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace CommandCenter
{
    [DataContract]
    public class QuestReward
    {
        [DataMember]
        string questName;
        [DataMember]
        string subquestNumber;



        public int index;
        public Village village;
        [DataMember]
        int wood, clay, iron, spearman, swordsman, archer, axeFighter, lightCavalry, mountedArcher, catapult;
        public QuestRewardBar bar;
        public void InitBar(int index, Village village)
        {
            this.index = index;
            this.village = village;
            bar = new QuestRewardBar(index, village);
            Type type = GetType();

            List<FieldInfo> fieldInfos = new List<FieldInfo>( type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));

            foreach (var f in fieldInfos)
            {
                string value = f.GetValue(this).ToString();
                if (value != "0")
                {
                    bar.textBlockQuantity.Text += f.Name + 'x'  + value + ' ';
                }
            }
        }
        //constructor to remove Warnings
        public QuestReward()
        {
            index = wood = clay = iron = spearman = swordsman = archer = axeFighter = lightCavalry = mountedArcher = catapult = 1;
            index = catapult;
        }
    }
}
