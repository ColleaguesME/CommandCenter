using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CommandCenter.Buildings;
using CommandCenter.Units;

namespace CommandCenter
{
    [DataContract]
    public class Order : IComparable<Order>
    {
        static int nextId = 1;
        [DataMember]
        double begin, end;
        DateTime _begin, _end;
        [DataMember]
        string name;
        [DataMember]
        int quantity, id;
        Order previous;
        Village village;
        public Entity entity;
        public OrderBar bar;
        public DateTime timerBuild, timerRecruit, timerResources;
        public List<PartialBarracksInfo> barracksInfos;
        public List<PartialFarmInfo> farmInfos;
        public List<PartialWaitInfo> waitInfos;
        public Dictionary<int, DateTime> barracksToDateTime;
        public Dictionary<int, int> barracksToLimit;

        public Resources resources;
        public int provisions;



        // ---------------Buildings
        public Headquarters headquarters;
        public TimberCamp timberCamp;
        public ClayPit clayPit;
        public IronMine ironMine;
        public Farm farm;
        public Warehouse warehouse;
        //Church | Chapel
        public RallyPoint rallyPoint;
        public Barracks barracks;
        public Statue statue;
        public Wall wall;
        public Hospital hospital;
        public Market market;
        public Tavern tavern;
        public Academy academy;
        public HallOfOrders hallOfOrders;

        //------------------------Units
        public Spearman spearman;
        public Swordsman swordsman;
        public Archer archer;
        public HeavyCavalry heavyCavalry;
        public AxeFighter axeFighter;
        public LightCavalry lightCavalry;
        public MountedArcher mountedArcher;
        public Ram ram;
        public Catapult catapult;

        public static Dictionary<string, Entity> stringToEntity;
        static int GetId()
        {
            return nextId++;
        }

        public Order(Village village, string name, int quantity = 1)
        {
            id = GetId();
            headquarters = new Headquarters();
            timberCamp = new TimberCamp();
            clayPit = new ClayPit();
            ironMine = new IronMine();
            farm = new Farm();
            warehouse = new Warehouse();
            rallyPoint = new RallyPoint();
            barracks = new Barracks();
            statue = new Statue();
            wall = new Wall();
            hospital = new Hospital();
            market = new Market();
            tavern = new Tavern();
            academy = new Academy();
            hallOfOrders = new HallOfOrders();

            spearman = new Spearman();
            swordsman = new Swordsman();
            archer = new Archer();
            heavyCavalry = new HeavyCavalry();
            axeFighter = new AxeFighter();
            lightCavalry = new LightCavalry();
            mountedArcher = new MountedArcher();
            ram = new Ram();
            catapult = new Catapult();


            barracksInfos = new List<PartialBarracksInfo>();
            farmInfos = new List<PartialFarmInfo>();
            waitInfos = new List<PartialWaitInfo>();
            barracksToDateTime = new Dictionary<int, DateTime>();
            stringToEntity = new Dictionary<string, Entity>()
            {
                {"Headquarters", headquarters },
                {"TimberCamp", timberCamp },
                {"ClayPit", clayPit },
                {"IronMine", ironMine },
                {"Farm", farm },
                {"Warehouse", warehouse },
                {"RallyPoint", rallyPoint },
                {"Barracks", barracks },
                {"Statue", statue },
                {"Wall", wall },
                {"Hospital", hospital },
                {"Market", market },
                {"Tavern", tavern },
                {"Academy", academy },
                {"HallOfOrders", hallOfOrders },

                {"Spearman", spearman },
                {"Swordsman", swordsman },
                {"Archer", archer },
                {"HeavyCavalry", heavyCavalry },
                {"AxeFighter", axeFighter },
                {"LightCavalry", lightCavalry },
                {"MountedArcher", mountedArcher },
                {"Ram", ram },
                {"Catapult", catapult },

                {"init" , null }
            };
            barracksToLimit = new Dictionary<int, int>()
            {
                {1, 5 },
                {2, 10 },
                {3, 15 }
            };
            for (int i = 4; i <= 25; i++ )
            {
                barracksToLimit.Add(i, int.MaxValue);
            }
            this.name = name;
            this.quantity = quantity;
            entity = stringToEntity[name];
            bar = new OrderBar(this.village = village);
        }
        public Order(Order toUpdate, Village village, string name, int quantity = 1) : this(village, name, quantity)
        {
            Update(toUpdate);
        }
        public void FixTime()
        {
            _begin = new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds(begin));
            _end = new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds(end));
        }
        public void SetShit(Order toSet)
        {
            Begin = toSet.Begin;
            End = toSet.End;
            Id = toSet.Id;
        }
        public void Update(Order toUpdate)
        {
            previous = toUpdate;
            resources = toUpdate.resources;
            provisions = toUpdate.provisions;

            timerBuild = toUpdate.timerBuild;
            timerRecruit = toUpdate.timerRecruit;
            timerResources = toUpdate.timerResources;

            headquarters.Update(toUpdate.headquarters);
            timberCamp.Update(toUpdate.timberCamp);
            clayPit.Update(toUpdate.clayPit);
            ironMine.Update(toUpdate.ironMine);
            farm.Update(toUpdate.farm);
            warehouse.Update(toUpdate.warehouse);
            rallyPoint.Update(toUpdate.rallyPoint);
            barracks.Update(toUpdate.barracks);
            statue.Update(toUpdate.statue);
            wall.Update(toUpdate.wall);
            hospital.Update(toUpdate.hospital);
            market.Update(toUpdate.market);
            tavern.Update(toUpdate.tavern);
            academy.Update(toUpdate.academy);
            hallOfOrders.Update(toUpdate.hallOfOrders);

            spearman.Update(toUpdate.spearman);
            swordsman.Update(toUpdate.swordsman);
            archer.Update(toUpdate.archer);
            heavyCavalry.Update(toUpdate.heavyCavalry);
            axeFighter.Update(toUpdate.axeFighter);
            lightCavalry.Update(toUpdate.lightCavalry);
            mountedArcher.Update(toUpdate.mountedArcher);
            ram.Update(toUpdate.ram);
            catapult.Update(toUpdate.catapult);

            barracksInfos = new List<PartialBarracksInfo>(toUpdate.barracksInfos);
            farmInfos = new List<PartialFarmInfo>(toUpdate.farmInfos);
            waitInfos = new List<PartialWaitInfo>(toUpdate.waitInfos);
            barracksToDateTime = new Dictionary<int, DateTime>(toUpdate.barracksToDateTime);
        }
        int GetWaitingTime(Resources expectedResources)
        {
            Resources resources = this.resources;
            DateTime timerResources = this.timerResources;
            int time = 0;
            if ((expectedResources - resources).GetWaitingTime(Production) > 0)
            {
                for (int i = WaitInfosCeilingIndex(timerResources); i < waitInfos.Count; i++)
                {
                    int newTime = (expectedResources - resources).GetWaitingTime(waitInfos[i].production);
                    DateTime next = i + 1 == waitInfos.Count ? DateTime.MaxValue : waitInfos[i + 1].time;
                    if (next > timerResources.AddSeconds(newTime))
                    {
                        time += newTime;
                        break;
                    }
                    else
                    {
                        newTime = Convert.ToInt32(Math.Ceiling((next - timerResources).TotalSeconds));
                        time += newTime;
                        resources += waitInfos[i].production * (newTime / Resources.DeltaTime);
                        resources.Ceil(waitInfos[i].capacity);
                        timerResources = timerResources.AddSeconds(newTime);
                    }
                }
            }
            return time;
        }
        public void Wait()
        {
            Wait(Begin);
            Spend(entity.GetResources() * quantity, entity.GetProvisions() * quantity);
        }
        void Wait(DateTime expectedTime)
        {
            for (int i = WaitInfosCeilingIndex(timerResources); i < waitInfos.Count; i++)
            {
                DateTime next = i + 1 == waitInfos.Count ? DateTime.MaxValue : waitInfos[i + 1].time;
                if (next > expectedTime)
                {
                    Wait(expectedTime, waitInfos[i].capacity, waitInfos[i].production);
                    break;
                }
                else
                {
                    Wait(next, waitInfos[i].capacity, waitInfos[i].production);
                }
            }
        }
        void Wait(DateTime expectedTime, int capacity, Resources production)
        {
            int time = Convert.ToInt32(Math.Ceiling((expectedTime - timerResources).TotalSeconds));
            if (time > 0)
            {
                resources += production * (time / Resources.DeltaTime);
                resources.Ceil(capacity);
                timerResources = timerResources.AddSeconds(time);
            }
        }
        void Spend(Resources resources, int provisions)
        {
            this.resources -= resources;
            this.resources.Floor();
            this.provisions += provisions;
        }
        public void Up()
        {
            bar.labelName.Content = name + ' ' + quantity;
            bar.labelBegin.Content = Begin.ToString();
            bar.labelEnd.Content = End.ToString();
            if (entity as Building != null)
            {
                timerBuild = End;
            }
            else if (entity as Unit != null)
            {
                timerRecruit = End;
            }
            double barracksSignificance = barracks.GetSignificance();
            entity.Up(quantity);
            if (entity as Barracks != null)
            {
                barracksToDateTime[barracks.Level] = End;
                if (barracksSignificance != barracks.GetSignificance())
                {
                    barracksInfos.Add(new PartialBarracksInfo(End, barracks.GetSignificance()));
                }
            }
            else if (entity as Farm != null)
            {
                farmInfos.Add(new PartialFarmInfo(timerBuild, farm.GetSignificance()));
            }
            else if (entity as Warehouse != null || entity as TimberCamp != null || entity as ClayPit != null || entity as IronMine != null)
            {
                waitInfos.Add(new PartialWaitInfo(timerResources, warehouse.GetSignificance(), Production));
            }
        }
        public List<Order> StringToOrders(Order order)
        {
            Wait(DateTime.UtcNow.AddMilliseconds(150));
            List<Order> orders = new List<Order>();
            order.Update(this);
            if (order.entity as Building != null)
            {
                //queue of buildings has 2 slots. We can drop resources only when at least one slot is avialable. 
                DateTime almostLastTimerBuild = DateTime.MinValue;
                bool found = false;
                Order almostLastOrder = order;
                while (almostLastOrder.previous != null)
                {
                    almostLastOrder = almostLastOrder.previous;
                    if (almostLastOrder.entity as Building != null)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    found = false;
                    while (almostLastOrder.previous != null)
                    {
                        almostLastOrder = almostLastOrder.previous;
                        if (almostLastOrder.entity as Building != null)
                        {
                            found = true;
                            almostLastTimerBuild = almostLastOrder.End;
                            break;
                        }
                    }
                }
                int farmIndex = FarmInfosCeilingIndex(timerResources);
                while (order.farmInfos[farmIndex].limit < order.entity.GetProvisions() + order.provisions)
                {
                    farmIndex++;
                }
                order.Begin = MaxDate(timerResources.AddSeconds(GetWaitingTime(order.entity.GetResources())), almostLastTimerBuild, farmInfos[farmIndex].time);
                order.Wait();
                order.End = MaxDate(order.Begin, order.timerBuild).AddSeconds(order.entity.GetTime());
                order.Up();
                orders.Add(order);
            }
            else if (order.entity as Unit != null)
            {
                int quantity = order.quantity;
                DateTime barracksDateTime = order.barracksToDateTime[order.entity.GetRequirements()],
                    currentFarm = MaxDate(barracksDateTime, order.timerResources),
                    nextFarm;
                int farmIndex = order.FarmInfosCeilingIndex(currentFarm);
                DateTime currentBarracks = currentFarm,
                    nextBarracks;
                int barracksIndex = order.BarracksInfosCeilingIndex(currentBarracks);
                while (quantity > 0)
                {
                    nextFarm = farmIndex + 1 >= order.farmInfos.Count ? DateTime.MaxValue : order.farmInfos[farmIndex + 1].time;

                    int limit = order.farmInfos[farmIndex].limit - order.provisions;

                    if (limit > 0)
                    {
                        int possibleQuantity = Math.Min(limit, quantity * order.entity.GetProvisions()),
                            timeResources = order.GetWaitingTime(order.entity.GetResources() * possibleQuantity),
                            timeRecruit = Convert.ToInt32(Math.Ceiling(order.entity.GetTime() * possibleQuantity * order.barracksInfos[barracksIndex].significance));
                        nextBarracks = barracksIndex + 1 >= order.barracksInfos.Count ? DateTime.MaxValue : order.barracksInfos[barracksIndex + 1].time;
                        if (nextBarracks < nextFarm)
                        {
                            double deltaTime = (nextBarracks - currentFarm).TotalSeconds;

                            while (possibleQuantity > 0)
                            {
                                if (deltaTime > timeResources + timeRecruit)
                                {
                                    break;
                                }
                                possibleQuantity--;
                                timeResources = order.GetWaitingTime(order.entity.GetResources() * possibleQuantity);
                                timeRecruit = Convert.ToInt32(Math.Ceiling(order.entity.GetTime() * possibleQuantity * order.barracksInfos[barracksIndex].significance));

                            }
                            currentBarracks = nextBarracks;
                            barracksIndex++;
                        }


                        if (possibleQuantity > 0)
                        {
                            int barracksLimit = barracksToLimit[order.barracks.Level];
                            quantity -= possibleQuantity;
                            do
                            {
                                int reallyPossibleQuantity = Math.Min(barracksLimit, possibleQuantity);
                                timeResources = order.GetWaitingTime(order.entity.GetResources() * reallyPossibleQuantity);
                                order.quantity = reallyPossibleQuantity;
                                order.Begin = MaxDate(order.timerResources.AddSeconds(timeResources), currentFarm);
                                order.Wait();
                                order.End = MaxDate(order.Begin, order.timerRecruit).AddSeconds(Convert.ToInt32(Math.Ceiling(order.entity.GetTime() * reallyPossibleQuantity * order.barracksInfos[barracksIndex].significance)));
                                order.Up();
                                orders.Add(order);
                                possibleQuantity -= reallyPossibleQuantity;
                                order = new Order(order, village, order.name, quantity);
                            } while (possibleQuantity > 0);
                        }
                        currentFarm = nextFarm;
                        farmIndex++;
                    }
                }
            }
            return orders;
        }
        public int GetPossibleQuantity(string name)
        {
            return (farm.GetSignificance() - provisions) / stringToEntity[name].GetProvisions();
        }

        int IComparable<Order>.CompareTo(Order other)
        {
            return begin.CompareTo(other.begin);
        }
        public DateTime Begin
        {
            get
            {
                return _begin;
            }
            set
            {
                _begin = value;
                begin = (_begin - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
        }
        public DateTime End
        {
            get
            {
                return _end;
            }
            set
            {
                _end = value;
                end = (_end - new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
            }
        }
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (nextId <= value)
                {
                    nextId = value + 1;
                }
                id = value;
            }
        }
        public Resources Production
        {
            get
            {
                return new Resources(timberCamp.GetSignificance(), clayPit.GetSignificance(), ironMine.GetSignificance());
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
        int FarmInfosCeilingIndex(DateTime date)
        {
            int index = farmInfos.BinarySearch(new PartialFarmInfo(date));
            if (index < 0)
            {
                index = -index - 2;
            }
            return index;
        }
        int BarracksInfosCeilingIndex(DateTime date)
        {
            int index = barracksInfos.BinarySearch(new PartialBarracksInfo(date));
            if (index < 0)
            {
                index = -index - 2;
            }
            return index;
        }
        int WaitInfosCeilingIndex(DateTime date)
        {
            int index = waitInfos.BinarySearch(new PartialWaitInfo(date));
            if (index < 0)
            {
                index = -index - 2;
            }
            return index;
        }
        double BarracksCeilingSignificance(DateTime date)
        {
            int index = barracksInfos.BinarySearch(new PartialBarracksInfo(date));
            if (index < 0)
            {
                index = -index - 2;
            }
            return barracksInfos[index].significance;
        }
        public bool IsHeadquartersAvailable()
        {
            return headquarters.GetResources() <= warehouse.GetSignificance() &&
                headquarters.GetProvisions() <= farm.GetSignificance() - provisions &&
                headquarters.IsNotMax();
        }
        public bool IsTimberCampAvailable()
        {
            return timberCamp.GetResources() <= warehouse.GetSignificance() &&
                timberCamp.GetProvisions() <= farm.GetSignificance() - provisions &&
                timberCamp.IsNotMax();
        }
        public bool IsClayPitAvailable()
        {
            return clayPit.GetResources() <= warehouse.GetSignificance() &&
                clayPit.GetProvisions() <= farm.GetSignificance() - provisions &&
                clayPit.IsNotMax();
        }
        public bool IsIronMineAvailable()
        {
            return ironMine.GetResources() <= warehouse.GetSignificance() &&
                ironMine.GetProvisions() <= farm.GetSignificance() - provisions &&
                ironMine.IsNotMax();
        }
        public bool IsFarmAvailable()
        {
            return farm.GetResources() <= warehouse.GetSignificance() &&
                farm.IsNotMax();
        }
        public bool IsWarehouseAvailable()
        {
            return warehouse.IsNotMax();
        }
        public bool IsRallyPointAvailable()
        {
            return rallyPoint.GetResources() <= warehouse.GetSignificance() &&
                rallyPoint.GetProvisions() <= farm.GetSignificance() - provisions &&
                rallyPoint.IsNotMax();
        }
        public bool IsBarracksAvailable()
        {
            return barracks.GetResources() <= warehouse.GetSignificance() &&
                barracks.GetProvisions() <= farm.GetSignificance() - provisions &&
                barracks.GetRequirements() <= headquarters.Level &&
                barracks.IsNotMax();
        }
        public bool IsStatueAvailable()
        {
            return statue.GetResources() <= warehouse.GetSignificance() &&
                statue.GetProvisions() <= farm.GetSignificance() - provisions &&
                statue.GetRequirements() <= headquarters.Level &&
                statue.IsNotMax();
        }
        public bool IsHospitalAvailable()
        {
            return hospital.GetResources() <= warehouse.GetSignificance() &&
                hospital.GetProvisions() <= farm.GetSignificance() - provisions &&
                hospital.GetRequirements() <= headquarters.Level &&
                hospital.IsNotMax();
        }
        public bool IsWallAvailable()
        {
            return wall.GetResources() <= warehouse.GetSignificance() &&
                wall.GetProvisions() <= farm.GetSignificance() - provisions &&
                wall.GetRequirements() <= headquarters.Level &&
                wall.IsNotMax();
        }
        public bool IsMarketAvailable()
        {
            return market.GetResources() <= warehouse.GetSignificance() &&
                market.GetProvisions() <= farm.GetSignificance() - provisions &&
                market.GetRequirements() <= headquarters.Level &&
                market.IsNotMax();
        }
        public bool IsTavernAvailable()
        {
            return tavern.GetResources() <= warehouse.GetSignificance() &&
                tavern.GetProvisions() <= farm.GetSignificance() - provisions &&
                tavern.GetRequirements() <= headquarters.Level &&
                tavern.IsNotMax();
        }
        public bool IsAcademyAvailable()
        {
            return academy.GetResources() <= warehouse.GetSignificance() &&
                academy.GetProvisions() <= farm.GetSignificance() - provisions &&
                academy.GetRequirements() <= headquarters.Level &&
                academy.IsNotMax();
        }
        public bool IsHallOfOrdersAvailable()
        {
            return hallOfOrders.GetResources() <= warehouse.GetSignificance() &&
                hallOfOrders.GetProvisions() <= farm.GetSignificance() - provisions &&
                hallOfOrders.GetRequirements() <= headquarters.Level &&
                hallOfOrders.IsNotMax();
        }
        public bool IsSpearmanAvailable()
        {
            return spearman.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsSwordsmanAvailable()
        {
            return swordsman.GetRequirements() <= barracks.Level &&
                swordsman.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsArcherAvailable()
        {
            return archer.GetRequirements() <= barracks.Level &&
                archer.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsHeavyCavalryAvailable()
        {
            return heavyCavalry.GetRequirements() <= barracks.Level &&
                heavyCavalry.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsAxeFighterAvailable()
        {
            return axeFighter.GetRequirements() <= barracks.Level &&
                axeFighter.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsLightCavalryAvailable()
        {
            return lightCavalry.GetRequirements() <= barracks.Level &&
                lightCavalry.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsMountedArcherAvailable()
        {
            return mountedArcher.GetRequirements() <= barracks.Level &&
                mountedArcher.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsRamAvailable()
        {
            return ram.GetRequirements() <= barracks.Level &&
                ram.GetProvisions() <= farm.GetSignificance() - provisions;
        }
        public bool IsCatapultAvailable()
        {
            return catapult.GetRequirements() <= barracks.Level &&
                catapult.GetProvisions() <= farm.GetSignificance() - provisions;
        }
    }
}
