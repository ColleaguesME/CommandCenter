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
    /// Interaction logic for MapMenuMate.xaml
    /// </summary>
    public partial class MapMenuMate : UserControl
    {
        VillageOnMap villageOnMap;
        public MapMenuMate(VillageOnMap villageOnMap)
        {
            this.villageOnMap = villageOnMap;
            InitializeComponent();
        }

        private void buttonShow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("show!");
        }

        private void buttonReinforce_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("reinforce!");
        }
    }
}
