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
    /// Interaction logic for Order.xaml
    /// </summary>
    public partial class OrderBar : UserControl
    {
        Village village;
        public int postiton;
        //TOREMOVE
        public OrderBar()
        {
            InitializeComponent();
        }
        public OrderBar(Village village)
        {
            InitializeComponent();
            this.village = village;
            labelName.Content += DateTime.UtcNow.ToLongTimeString();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            village.RemoveOrder(postiton);
        }
    }
}
