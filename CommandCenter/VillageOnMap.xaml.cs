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
    /// Interaction logic for VillageOnMap.xaml
    /// </summary>
    public partial class VillageOnMap : UserControl
    {
        public const int Size = 25;

        static List<int> pointsTable = new List<int>()
        {
            {100 },
            {400 },
            {1000 },
            {2000 },
            {3000 },
            {4000 },
            {5000 },
            {6000 },
            {7000 },
            {8000 },
            {9000 },
            {10000 },
            {11000 },
        };

        public int x, y;
        int points;
        public string name { get; set;}
        string player, province, tribe; 
        public Village village
        {
            get
            {
                return village;
            }
            set
            {
                village = value;
                label.Foreground = Brushes.Green;
            }
        }
        
        UserControl mapMenu;

        public VillageOnMap(int x, int y, string name, string province, string player, string tribe, int points)
        {
            this.x = x;
            this.y = y;
            this.name = name;
            this.province = province;
            this.player = player;
            this.tribe = tribe;
            this.points = points;

            mapMenu = new MapMenuFoe(this);

            InitializeComponent();

            grid.Height = grid.Width = Size;
            Label label = new Label();
            label.Content = GetCharIcon().ToString();
            label.Foreground = player == "Barbarian" ? Brushes.LightGray : Brushes.Red;
            grid.Children.Add(label);
            grid.MouseUp += VillageOnMap_MouseUp;


        }

        public void SetCoords()
        {
            Canvas.SetLeft(this, (y%2 == 0 ? x : x + 0.5) * Size - MainWindow.Current.currentX);
            Canvas.SetTop(this, y * Size - MainWindow.Current.currentY);
        }
        private void VillageOnMap_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!MainWindow.Current.moved)
            {
                MainWindow.Current.canvasMap.Children.Add(mapMenu);
                Canvas.SetLeft(mapMenu, (y % 2 == 0 ? x : x + 0.5) * Size - MainWindow.Current.currentX + Size);
                Canvas.SetTop(mapMenu, y * Size - MainWindow.Current.currentY + Size);
                MainWindow.Current.mapMenu = mapMenu;
            }
        }
        private char GetCharIcon()
        {
            int index = pointsTable.BinarySearch(points);
            if (index < 0)
            {
                index = -index - 2;
            }
            return Convert.ToChar(66 + index);
        }

    }
}
