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
using System.Windows.Input;

namespace CommandCenter
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class Village
    {
        public KachBar Bar;
        public KachTab tab;
        public TabItem tabItem;
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
            tab = new KachTab(this);

            tabItem.Header = tab.Village.Name;
            tabItem.KeyDown += MainWindow.Current.CloseTab;
            tabItem.Content = tab;
            Bar = new KachBar(tabItem);
            village.Bar.MouseDoubleClick += OpenTab;
            name = village.name;
            Update(village);
        }
        public void OpenTab(object sender, RoutedEventArgs e)
        {
            TabItem tabItem = (sender as KachBar).tabItem;
            MainWindow.Current.tabControl.Items.Add(tabItem);
            MainWindow.Current.tabControl.SelectedIndex = MainWindow.Current.tabControl.Items.IndexOf(tabItem);
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
            
            account.socket.Send(new OrderChange(position - 1, "add", orders.GetRange(position, orders.Count - position)).GetBytes());
            DisplayOrders();
        }
        public void RemoveAllOrders()
        {
            //rewrite
            position = 0;
            orders.RemoveRange(1, orders.Count - 1);
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

                account.socket.Send(new OrderChange(toRemove - 1, "cancel", orders.GetRange(toRemove, orders.Count - toRemove)).GetBytes());

                DisplayOrders();
            }
        }
        public int GetPossibleQuantity(string name)
        {
            return orders[Position].GetPossibleQuantity(name);
        }
        public void Update(Village toUpdate)
        {
            tab.buttonTakeSmallReward.IsEnabled = account.smallRewardQuantity > 0;
            


            Order order = new Order(this, "init");

            order.timerBuild = DateTime.UtcNow.AddMilliseconds(100);
            order.timerRecruit = DateTime.UtcNow.AddMilliseconds(100);
            order.timerResources = DateTime.UtcNow.AddMilliseconds(100);
            order.provisions = provisions = Farm.GetSignificance(toUpdate.farm) - toUpdate.provisions;
            order.resources = resources = toUpdate.resources;

            tab.labelWood.Content = Bar.labelWood.Content = resources.Wood.ToString();
            tab.labelClay.Content = Bar.labelClay.Content = resources.Clay.ToString();
            tab.labelIron.Content = Bar.labelIron.Content = resources.Iron.ToString();
            tab.labelProvisions.Content = Bar.labelProvisions.Content = provisions.ToString() + '/' + Farm.GetSignificance(toUpdate.farm);


            tab.labelHeadquarters.Content = Bar.labelHeadquarters.Content = (order.headquarters.Level = headquarters = toUpdate.headquarters).ToString();
            tab.labelTimberCamp.Content = Bar.labelTimberCamp.Content = (order.timberCamp.Level = timberCamp = toUpdate.timberCamp).ToString();
            tab.labelClayPit.Content = Bar.labelClayPit.Content = (order.clayPit.Level = clayPit = toUpdate.clayPit).ToString();
            tab.labelIronMine.Content = Bar.labelIronMine.Content = (order.ironMine.Level = ironMine = toUpdate.ironMine).ToString();
            tab.labelFarm.Content = Bar.labelFarm.Content = (order.farm.Level = farm = toUpdate.farm).ToString();
            tab.labelWarehouse.Content = Bar.labelWarehouse.Content = (order.warehouse.Level = warehouse = toUpdate.warehouse).ToString();
            tab.labelRallyPoint.Content = (order.rallyPoint.Level = rallyPoint = toUpdate.rallyPoint).ToString();
            tab.labelBarracks.Content = Bar.labelBarracks.Content = (order.barracks.Level = barracks = toUpdate.barracks).ToString();
            tab.labelStatue.Content = (order.statue.Level = statue = toUpdate.statue).ToString();
            tab.labelWall.Content = Bar.labelWall.Content = (order.wall.Level = wall = toUpdate.wall).ToString();
            tab.labelHospital.Content = (order.hospital.Level = hospital = toUpdate.hospital).ToString();
            tab.labelMarket.Content = (order.market.Level = market = toUpdate.market).ToString();
            tab.labelTavern.Content = (order.tavern.Level = tavern = toUpdate.tavern).ToString();
            tab.labelAcademy.Content = (order.academy.Level = academy = toUpdate.tavern).ToString();
            tab.labelHallOfOrders.Content = (order.hallOfOrders.Level = hallOfOrders = toUpdate.hallOfOrders).ToString();

            tab.labelSpearman.Content = Bar.labelSpearman.Content = (order.spearman.Quantity = spearman = toUpdate.spearman).ToString();
            tab.labelSwordsman.Content = Bar.labelSwordsman.Content = (order.swordsman.Quantity = swordsman = toUpdate.swordsman).ToString();
            tab.labelArcher.Content = Bar.labelArcher.Content = (order.archer.Quantity = archer = toUpdate.archer).ToString();
            tab.labelHeavyCavalry.Content = Bar.labelHeavyCavalry.Content = (order.heavyCavalry.Quantity = heavyCavalry = toUpdate.heavyCavalry).ToString();
            tab.labelAxeFighter.Content = Bar.labelAxeFighter.Content = (order.axeFighter.Quantity = axeFighter = toUpdate.axeFighter).ToString();
            tab.labelLightCavalry.Content = Bar.labelLightCavalry.Content = (order.lightCavalry.Quantity = lightCavalry = toUpdate.lightCavalry).ToString();
            tab.labelMountedArcher.Content = Bar.labelMountedArcher.Content = (order.mountedArcher.Quantity = mountedArcher = toUpdate.mountedArcher).ToString();
            tab.labelRam.Content = Bar.labelRam.Content = (order.ram.Quantity = ram = toUpdate.ram).ToString();
            tab.labelCatapult.Content = Bar.labelCatapult.Content = (order.catapult.Quantity = catapult = toUpdate.catapult).ToString();

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
            DisplayQuestRewards();
        }
        public void DisplayOrders()
        {
            tab.SetOrderBars(orders);
            tab.Select(position);
        }
        public void DisplayQuestRewards()
        {
            tab.stackPanelQuestRewards.Children.RemoveRange(1, tab.stackPanelQuestRewards.Children.Count - 1);
            for (int i = 0; i < (account.questRewards == null ? 0 : account.questRewards.Count); i++)
            {
                account.questRewards[i].InitBar(i, this);
                tab.stackPanelQuestRewards.Children.Add(account.questRewards[i].bar);
            }
        }
        public void RefreshButtons(int position)
        {
            this.position = position;
            Order order = orders[position];

            tab.labelVirtualWood.Content = order.resources.Wood.ToString();
            tab.labelVirtualClay.Content = order.resources.Clay.ToString();
            tab.labelVirtualIron.Content = order.resources.Iron.ToString();
            tab.labelVirtualProvisions.Content = order.provisions.ToString() + '/' + Farm.GetSignificance(order.farm.Level);


            tab.buttonHeadquarters.IsEnabled = order.IsHeadquartersAvailable();

            tab.buttonTimberCamp.IsEnabled = order.IsTimberCampAvailable();

            tab.buttonClayPit.IsEnabled = order.IsClayPitAvailable();

            tab.buttonIronMine.IsEnabled = order.IsIronMineAvailable();

            tab.buttonFarm.IsEnabled = order.IsFarmAvailable();

            tab.buttonWarehouse.IsEnabled = order.IsWarehouseAvailable();

            //TODO church button

            tab.buttonRallyPoint.IsEnabled = order.IsRallyPointAvailable();

            tab.buttonBarracks.IsEnabled = order.IsBarracksAvailable();

            tab.buttonStatue.IsEnabled = order.IsStatueAvailable();

            tab.buttonHospital.IsEnabled = order.IsHospitalAvailable();

            tab.buttonWall.IsEnabled = order.IsWallAvailable();

            tab.buttonMarket.IsEnabled = order.IsMarketAvailable();

            tab.buttonTavern.IsEnabled = order.IsTavernAvailable();

            tab.buttonAcademy.IsEnabled = order.IsAcademyAvailable();

            tab.buttonHallOfOrders.IsEnabled = order.IsHallOfOrdersAvailable();


            tab.buttonSpearman.IsEnabled = order.IsSpearmanAvailable();

            tab.buttonSwordsman.IsEnabled = order.IsSwordsmanAvailable();

            tab.buttonArcher.IsEnabled = order.IsArcherAvailable();

            tab.buttonHeavyCavalry.IsEnabled = order.IsHeavyCavalryAvailable();

            tab.buttonAxeFighter.IsEnabled = order.IsAxeFighterAvailable();

            tab.buttonLightCavalry.IsEnabled = order.IsLightCavalryAvailable();

            tab.buttonMountedArcher.IsEnabled = order.IsMountedArcherAvailable();

            tab.buttonRam.IsEnabled = order.IsRamAvailable();

            tab.buttonCatapult.IsEnabled = order.IsCatapultAvailable();
                

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
