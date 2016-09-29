using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CommandCenter
{
    /// <summary>
    /// Interaction logic for AttackTab.xaml
    /// </summary>
    public partial class AttackTab : UserControl
    {
        VillageOnMap aimedVillage;
        Dictionary<string, bool> unitsState = new Dictionary<string, bool>()
        {
            {"Spearman", false },
            {"Swordsman", false },
            {"Archer", false },
            {"HeavyCavalry", false },
            {"AxeFighter", false },
            {"LightCavalry", false },
            {"MountedArcher", false },
            {"Ram", false },
            {"Catapult", false }
        };
        Dictionary<string, int> unitsSpeed = new Dictionary<string, int>()
        {
            {"Spearman", 14 },
            {"Swordsman", 18 },
            {"Archer", 14 },
            {"HeavyCavalry", 9 },
            {"AxeFighter", 14 },
            {"LightCavalry", 8 },
            {"MountedArcher", 8 },
            {"Ram", 24 },
            {"Catapult", 24 }
        };

        public AttackTab(VillageOnMap aimedVillage)
        {
            InitializeComponent();
            this.aimedVillage = aimedVillage;
        }

        private void toggleUnits(object sender, MouseButtonEventArgs e)
        {
            Border toggled = (Border)sender;
            if (unitsState[toggled.Name.Substring(6)])
            {
                unitsState[toggled.Name.Substring(6)] = false;
                toggled.BorderBrush = Brushes.Blue;
            }
            else
            {
                unitsState[toggled.Name.Substring(6)] = true;
                toggled.BorderBrush = Brushes.Red;
            }
        }

        private void Calculate(object sender, MouseButtonEventArgs e)
        {
            DateTime desiredTimeOfAttack;
            try
            {
                desiredTimeOfAttack = DateTime.Parse(textBoxDesiredTime.Text);
            }
            catch {

                textBoxDesiredTime.BorderBrush = Brushes.Red;
                return;
            }
            int minUnitSpeed = unitsState.Where(us => us.Value == true).Min(us => unitsSpeed[us.Key]);
            MainWindow.Current.accounts.ForEach(acc => acc.villages.ForEach(aliedVillage => {
                double distance = Math.Sqrt((aimedVillage.x - aliedVillage.x) * 2 + (aimedVillage.y - aliedVillage.y) * 2);
                double timeOfAttack = distance * minUnitSpeed;

            }));
        }

        private void sendArmy(object sender, MouseButtonEventArgs e)
        {

        }

        private void changeToNormalState(object sender, TextChangedEventArgs e)
        {
            textBoxDesiredTime.BorderBrush = Brushes.Gray;
        }
    }
}
