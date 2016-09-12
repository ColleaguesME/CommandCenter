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
    /// Interaction logic for KachTab.xaml
    /// </summary>
    public partial class KachTab : UserControl
    {
        public Village Village;
        public bool IsOpened;
        UIElementCollection orderElements;
        List<PositionPanel> positionPanels;
        PositionPanel selectedPosition;
        public KachTab(Village Village)
        {
            this.Village = Village;
            IsOpened = false;
            InitializeComponent();
            orderElements = stackPanelOrders.Children;
            positionPanels = new List<PositionPanel>()
            {
                zeroPosition
            };
        }
        public void SetOrderBars(List<Order> orders)
        {
            RemoveOrders();
            //orders[0] is fake
            for (int i = 1; i < orders.Count; i++)
            {
                PositionPanel positionPanel = new PositionPanel();
                orders[i].bar.index = i;
                orderElements.Add(orders[i].bar);
                orderElements.Add(positionPanel);
                positionPanels.Add(positionPanel);
            }
            Select(Village.Position);
        }
        void RemoveOrders()
        {
            if (orderElements.Count >= 2)
            {
                orderElements.RemoveRange(2, orderElements.Count - 2);
                positionPanels.RemoveRange(1, positionPanels.Count - 1);
            }
        }
        private void Upgrade(object sender, RoutedEventArgs e)
        {
            Village.AddOrder((sender as Button).Name.Substring(6));
        }

        private void Recruit(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button).Name.Substring(6);
            TextBox textBox = ((sender as FrameworkElement).Parent as Panel).Children.OfType<TextBox>().First();
            int quantity = int.Parse(textBox.Text),
                possibleQuantity = Village.GetPossibleQuantity(name);
            if (quantity > possibleQuantity)
            {
                textBox.BorderBrush = Brushes.Red;
                textBox.Text = possibleQuantity.ToString();
            }
            else
            {
                //default color
                Color color = new Color();
                color.A = 0xFF;
                color.R = 0x70;
                color.G = 0x70;
                color.B = 0x70;
                textBox.BorderBrush = new SolidColorBrush(color);

                Village.AddOrder(name, quantity);
            }
        }
        public void Select(int position)
        {
            if (selectedPosition != null)
            {
                selectedPosition.Select(false);
            }
            selectedPosition = positionPanels[position];
            selectedPosition.Select(true);
            Village.RefreshButtons(position);
        }

        private void stackPanelOrders_MouseUp(object sender, MouseButtonEventArgs e)
        {
            double y = e.GetPosition(sender as IInputElement).Y;
            int position = Convert.ToInt32(Math.Round(y)) / 60;
            Select(position);
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            byte[] message = Encoding.UTF8.GetBytes("Get Information");
            Village.account.socket.Send(message);
        }

        private void buttonTakeSmallReward_Click(object sender, RoutedEventArgs e)
        {
            byte[] message = Encoding.UTF8.GetBytes("Take Small Reward");
            Village.account.socket.Send(message);
        }

        private void buttonRemoveOrders_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.Yes == MessageBox.Show("Are you sure?", "Removing all orders", MessageBoxButton.YesNo))
            {
                Village.RemoveAllOrders();
            }
        }
    }
}
