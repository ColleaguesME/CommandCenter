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
    /// Interaction logic for QuestReward.xaml
    /// </summary>
    public partial class QuestRewardBar : UserControl
    {
        int index;
        Village village;
        public QuestRewardBar(int index, Village village)
        {
            this.index = index;
            this.village = village;
            InitializeComponent();
        }

        private void buttonClaim_Click(object sender, RoutedEventArgs e)
        {
            //village.ClaimReward(index);
        }
    }
}
