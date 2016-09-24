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
    /// Interaction logic for MapMenuFoe.xaml
    /// </summary>
    public partial class MapMenuFoe : UserControl
    {
        VillageOnMap villageOnMap;
        public MapMenuFoe(VillageOnMap villageOnMap)
        {
            this.villageOnMap = villageOnMap;
            InitializeComponent();
        }

        private void buttonShow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("show!");
        }

        private void buttonSpy_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("spy!");
        }

        private void buttonAttack_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("attack!");
            AttackTab attackTab = new AttackTab();
            TabItem item = new TabItem();
            item.Header = "Attack "+villageOnMap.name;
        }
    }
}
