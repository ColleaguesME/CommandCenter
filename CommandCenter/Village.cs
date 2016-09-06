using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommandCenter.Buildings;
using CommandCenter.Units;

namespace CommandCenter
{
    [DataContract]
    public class Village
    {
        public KachBar Bar;
        public KachTab Tab;
        public AccountInformation account;
        [DataMember]
        string name;
        [DataMember]
        Resources resources;
        [DataMember]
        int provisions;
        [DataMember]
        List<Order> orders;
        [DataMember]
        List<QuestReward> questRewards;
        [DataMember]
        int smallRewardQuantity;
        [DataMember]
        public int x, y;


        [DataMember]
        int headquarters, timberCamp, clayPit, ironMine,
            farm, warehouse, rallyPoint, barracks, statue, wall,
            hospital, market, tavern, academy, hallOfOrders,
            spearman, swordsman, archer, heavyCavalry, axeFighter,
            lightCavalry, mountedArcher, ram, catapult;

        int position;

        public Village(Village village, AccountInformation account)
        {
            this.account = account;
            Tab = new KachTab(this);
            Bar = new KachBar(Tab);
            name = village.name;
            Update(village);
        }
        public void AddSingleOrder(string name, int quantity = 1)
        {
            AddOrder(name, quantity);
            SendVilalge();
            DisplayOrders();
        }
        public void AddOrdersFromFile(string path)
        {
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader(path);
                string[] buffer = reader.ReadLine().Split(' ');
                int quantity = int.Parse(buffer[1]);
                //if (buffer)
            }
        }
        public void AddOrder(string name, int quantity = 1)
        {
            int realPlace = position + 1;
            List<Order> toHandle;
            if (realPlace >= orders.Count)
            {
                toHandle = new List<Order>();
            }
            else
            {
                toHandle = orders.GetRange(realPlace, orders.Count - realPlace);
                orders.RemoveRange(realPlace, orders.Count - realPlace);
            }
            List<Order> temp = orders[orders.Count - 1].StringToOrders(new Order(this, name, quantity));
            position += temp.Count;
            orders.AddRange(temp);
            for (int i = 0; i < toHandle.Count; i++)
            {
                orders.AddRange(orders[orders.Count - 1].StringToOrders(toHandle[i]));
            }
            SendVilalge();
            DisplayOrders();
        }
        public void RemoveAllOrders()
        {
            position = 0;
            orders.RemoveRange(1, orders.Count - 1);
            SendVilalge();
            DisplayOrders();
        }
        public void RemoveOrder(int toRemove)
        {
            if (orders[toRemove].End > DateTime.UtcNow.AddSeconds(4))
            {
                int next = toRemove + 1;
                List<Order> toHandle;
                if (next >= orders.Count)
                {
                    toHandle = new List<Order>();
                }
                else
                {
                    toHandle = orders.GetRange(next, orders.Count - next);
                    orders.RemoveRange(next, orders.Count - next);
                }
                if (orders[toRemove].Begin < DateTime.UtcNow)
                {
                    orders[toRemove - 1].resources -= orders[toRemove].entity.GetResources() * 0.1;
                }
                orders.RemoveAt(toRemove);
                foreach (Order order in toHandle)
                {
                    orders.AddRange(orders[orders.Count - 1].StringToOrders(order));
                }
                if (position >= toRemove)
                {
                    position--;
                }
                SendVilalge();
                DisplayOrders();
            }
        }

        void SendVilalge()
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Village));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, this);
            byte[] buffer = stream.ToArray();
            //byte[] buffer = Encoding.UTF8.GetBytes(Serializer.Get().Serialize(this));
            if (!account.isClosed)
            {
                account.socket.Send(buffer);
            }
        }
        public int GetPossibleQuantity(string name)
        {
            return orders[Position].GetPossibleQuantity(name);
        }
        public void Update(Village toUpdate)
        {
            smallRewardQuantity = toUpdate.smallRewardQuantity;
            Tab.buttonTakeSmallReward.IsEnabled = smallRewardQuantity > 0;
            


            Order order = new Order(this, "init");

            order.timerBuild = DateTime.UtcNow.AddMilliseconds(100);
            order.timerRecruit = DateTime.UtcNow.AddMilliseconds(100);
            order.timerResources = DateTime.UtcNow.AddMilliseconds(100);
            order.provisions = provisions = Farm.GetSignificance(toUpdate.farm) - toUpdate.provisions;
            order.resources = resources = toUpdate.resources;

            Tab.labelWood.Content = Bar.labelWood.Content = resources.Wood.ToString();
            Tab.labelClay.Content = Bar.labelClay.Content = resources.Clay.ToString();
            Tab.labelIron.Content = Bar.labelIron.Content = resources.Iron.ToString();
            Tab.labelProvisions.Content = Bar.labelProvisions.Content = provisions.ToString() + '/' + Farm.GetSignificance(toUpdate.farm);


            Tab.labelHeadquarters.Content = Bar.labelHeadquarters.Content = (order.headquarters.Level = headquarters = toUpdate.headquarters).ToString();
            Tab.labelTimberCamp.Content = Bar.labelTimberCamp.Content = (order.timberCamp.Level = timberCamp = toUpdate.timberCamp).ToString();
            Tab.labelClayPit.Content = Bar.labelClayPit.Content = (order.clayPit.Level = clayPit = toUpdate.clayPit).ToString();
            Tab.labelIronMine.Content = Bar.labelIronMine.Content = (order.ironMine.Level = ironMine = toUpdate.ironMine).ToString();
            Tab.labelFarm.Content = Bar.labelFarm.Content = (order.farm.Level = farm = toUpdate.farm).ToString();
            Tab.labelWarehouse.Content = Bar.labelWarehouse.Content = (order.warehouse.Level = warehouse = toUpdate.warehouse).ToString();
            Tab.labelRallyPoint.Content = (order.rallyPoint.Level = rallyPoint = toUpdate.rallyPoint).ToString();
            Tab.labelBarracks.Content = Bar.labelBarracks.Content = (order.barracks.Level = barracks = toUpdate.barracks).ToString();
            Tab.labelStatue.Content = (order.statue.Level = statue = toUpdate.statue).ToString();
            Tab.labelWall.Content = Bar.labelWall.Content = (order.wall.Level = wall = toUpdate.wall).ToString();
            Tab.labelHospital.Content = (order.hospital.Level = hospital = toUpdate.hospital).ToString();
            Tab.labelMarket.Content = (order.market.Level = market = toUpdate.market).ToString();
            Tab.labelTavern.Content = (order.tavern.Level = tavern = toUpdate.tavern).ToString();
            Tab.labelAcademy.Content = (order.academy.Level = academy = toUpdate.tavern).ToString();
            Tab.labelHallOfOrders.Content = (order.hallOfOrders.Level = hallOfOrders = toUpdate.hallOfOrders).ToString();

            Tab.labelSpearman.Content = Bar.labelSpearman.Content = (order.spearman.Quantity = spearman = toUpdate.spearman).ToString();
            Tab.labelSwordsman.Content = Bar.labelSwordsman.Content = (order.swordsman.Quantity = swordsman = toUpdate.swordsman).ToString();
            Tab.labelArcher.Content = Bar.labelArcher.Content = (order.archer.Quantity = archer = toUpdate.archer).ToString();
            Tab.labelHeavyCavalry.Content = Bar.labelHeavyCavalry.Content = (order.heavyCavalry.Quantity = heavyCavalry = toUpdate.heavyCavalry).ToString();
            Tab.labelAxeFighter.Content = Bar.labelAxeFighter.Content = (order.axeFighter.Quantity = axeFighter = toUpdate.axeFighter).ToString();
            Tab.labelLightCavalry.Content = Bar.labelLightCavalry.Content = (order.lightCavalry.Quantity = lightCavalry = toUpdate.lightCavalry).ToString();
            Tab.labelMountedArcher.Content = Bar.labelMountedArcher.Content = (order.mountedArcher.Quantity = mountedArcher = toUpdate.mountedArcher).ToString();
            Tab.labelRam.Content = Bar.labelRam.Content = (order.ram.Quantity = ram = toUpdate.ram).ToString();
            Tab.labelCatapult.Content = Bar.labelCatapult.Content = (order.catapult.Quantity = catapult = toUpdate.catapult).ToString();

            order.barracksInfos.Add(new PartialBarracksInfo(DateTime.UtcNow, order.barracks.GetSignificance()));
            order.farmInfos.Add(new PartialFarmInfo(DateTime.UtcNow, order.farm.GetSignificance()));
            order.waitInfos.Add(new PartialWaitInfo(DateTime.UtcNow, order.warehouse.GetSignificance(), order.Production));

            order.barracksToDateTime = new Dictionary<int, DateTime>();
            int i;
            for (i = 0; i <= 25; ++i)
            {
                order.barracksToDateTime[i] = DateTime.MinValue;
            }
            orders = new List<Order>();
            orders.Add(order);
            for (i = 1; i < (toUpdate.orders == null ? 0 : toUpdate.orders.Count + 1); i++)
            {
                toUpdate.orders[i - 1].FixTime();
                if (toUpdate.orders[i - 1].End < DateTime.UtcNow)
                {
                    continue;
                }
                else
                {
                    orders.Add(new Order(orders[i - 1], this, toUpdate.orders[i - 1].Name, toUpdate.orders[i - 1].Quantity));
                    orders[i].SetShit(toUpdate.orders[i - 1]);
                    if (orders[i].Begin >= DateTime.UtcNow)
                    {
                        orders[i].Wait();
                    }
                    orders[i].Up();
                }
            }
            if (position >= orders.Count )
            {
                position = orders.Count - 1;
            }
            DisplayOrders();
            questRewards = toUpdate.questRewards;
            DisplayQuestRewards();
        }
        public void DisplayOrders()
        {
            Tab.SetOrderBars(orders);
            Tab.Select(position);
        }
        public void DisplayQuestRewards()
        {
            Tab.stackPanelQuestRewards.Children.RemoveRange(1, Tab.stackPanelQuestRewards.Children.Count - 1);
            for (int i = 0; i < (questRewards == null ? 0 : questRewards.Count); i++)
            {
                questRewards[i].InitBar(i, this);
                Tab.stackPanelQuestRewards.Children.Add(questRewards[i].bar);
            }
        }
        public void ClaimReward(int index)
        {
            questRewards.RemoveAt(index);
            SendVilalge();
            DisplayQuestRewards();
        }
        public void RefreshButtons(int position)
        {
            this.position = position;
            Order order = orders[position];

            Tab.labelVirtualWood.Content = order.resources.Wood.ToString();
            Tab.labelVirtualClay.Content = order.resources.Clay.ToString();
            Tab.labelVirtualIron.Content = order.resources.Iron.ToString();
            Tab.labelVirtualProvisions.Content = order.provisions.ToString() + '/' + Farm.GetSignificance(order.farm.Level);


            Tab.buttonHeadquarters.IsEnabled = order.IsHeadquartersAvailable();

            Tab.buttonTimberCamp.IsEnabled = order.IsTimberCampAvailable();

            Tab.buttonClayPit.IsEnabled = order.IsClayPitAvailable();

            Tab.buttonIronMine.IsEnabled = order.IsIronMineAvailable();

            Tab.buttonFarm.IsEnabled = order.IsFarmAvailable();

            Tab.buttonWarehouse.IsEnabled = order.IsWarehouseAvailable();

            //TODO church button

            Tab.buttonRallyPoint.IsEnabled = order.IsRallyPointAvailable();

            Tab.buttonBarracks.IsEnabled = order.IsBarracksAvailable();

            Tab.buttonStatue.IsEnabled = order.IsStatueAvailable();

            Tab.buttonHospital.IsEnabled = order.IsHospitalAvailable();

            Tab.buttonWall.IsEnabled = order.IsWallAvailable();

            Tab.buttonMarket.IsEnabled = order.IsMarketAvailable();

            Tab.buttonTavern.IsEnabled = order.IsTavernAvailable();

            Tab.buttonAcademy.IsEnabled = order.IsAcademyAvailable();

            Tab.buttonHallOfOrders.IsEnabled = order.IsHallOfOrdersAvailable();


            Tab.buttonSpearman.IsEnabled = order.IsSpearmanAvailable();

            Tab.buttonSwordsman.IsEnabled = order.IsSwordsmanAvailable();

            Tab.buttonArcher.IsEnabled = order.IsArcherAvailable();

            Tab.buttonHeavyCavalry.IsEnabled = order.IsHeavyCavalryAvailable();

            Tab.buttonAxeFighter.IsEnabled = order.IsAxeFighterAvailable();

            Tab.buttonLightCavalry.IsEnabled = order.IsLightCavalryAvailable();

            Tab.buttonMountedArcher.IsEnabled = order.IsMountedArcherAvailable();

            Tab.buttonRam.IsEnabled = order.IsRamAvailable();

            Tab.buttonCatapult.IsEnabled = order.IsCatapultAvailable();
                

        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public int Position
        {
            get
            {
                return position;
            }
        }
        static DateTime MaxDate(DateTime first, DateTime second)
        {
            return first > second ? first : second;
        }
        static DateTime MaxDate(DateTime first, DateTime second, DateTime third)
        {
            DateTime max = MaxDate(second, third);
            return first > max ? first : max;
        }
        static DateTime MaxDate(DateTime first, DateTime second, DateTime third, DateTime fourth)
        {
            DateTime max12 = MaxDate(first, second),
                max34 = MaxDate(third, fourth);
            return max12 > max34 ? max12 : max34;
        }
    }
}
