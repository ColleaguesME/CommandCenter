using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

using MySql;
using MySql.Data;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using MySql.Fabric;

namespace CommandCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current;
        public List<AccountInformation> accounts;
        public List<VillageOnMap> villagesOnMap;
        public double currentX, currentY;
        public UserControl mapMenu;

        MySqlConnection mySqlConnection;
        MySqlDataReader mySqlDataReader;

        int canvasFirstIcon;

        public MainWindow()
        {
            Current = this;
            mySqlConnection = new MySqlConnection("server=127.0.0.1;uid=root;pwd=pavellev7;database=db;");
            mySqlConnection.Open();
            InitializeComponent();
            canvasFirstIcon = canvasMap.Children.Count;
            GetMyIp();
            villagesOnMap = new List<VillageOnMap>();
            accounts = new List<AccountInformation>();
            MoveAllTo(500, 500);
            MySqlCommand mySqlCommand = new MySqlCommand(@"
                select vi.x, vi.y, vi.name, vi.province, vi.player, vi.tribe, vi.points
                from villageinfos vi inner
                join (
                    select x, y, max(date) as maxdate
                    from villageinfos
                    group by x, y
                ) md
                on vi.x = md.x and vi.y = md.y and vi.date = md.maxdate; ", mySqlConnection);
            mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                VillageOnMap villageOnMap = new VillageOnMap(mySqlDataReader.GetInt32(0), mySqlDataReader.GetInt32(1),
                    mySqlDataReader.GetString(2), mySqlDataReader.GetString(3), mySqlDataReader.GetString(4),
                    mySqlDataReader.GetString(5), mySqlDataReader.GetInt32(6));
                villagesOnMap.Add(villageOnMap);
            }
            ShowVisible();
        }

        public void GetMyIp()
        {
            Process process = new Process();
            string ip = "";
            process.StartInfo.FileName = "nslookup";
            process.StartInfo.Arguments = "myip.opendns.com. resolver1.opendns.com";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Exited += new EventHandler((sndr, eargs) =>
            {
                ip = (sndr as Process).StandardOutput.ReadToEnd();
                ip = ip.Substring(ip.LastIndexOf(" ") + 1);
                ip = ip.Remove(ip.Length - 4);

                AccountInformation.myIp = ip;

                Dispatcher.Invoke(() =>
                {
                    buttonReadAccounts.IsEnabled = true;
                });
            });
            process.EnableRaisingEvents = true;
            process.Start();
        }

        private void ReadAccounts(object sender, RoutedEventArgs e)
        {
            string[] lines = System.IO.File.ReadAllLines(textBoxPath.Text);
            foreach (string line in lines)
            {
                accounts.Add(new AccountInformation(line));
            }
            tabControl.SelectedIndex = 3;
            tabControl.Items.Remove(tabItemStart);
        }
        public void CloseTab(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.W) && ((Keyboard.Modifiers & ModifierKeys.Control) > 0))
            {
                tabControl.Items.Remove(sender);
            }
        }

        bool dragging;
        public bool moved;
        Point previousPoint, currentPoint, startPoint;
        int x, y;


        private void imageBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            dragging = true;
            moved = false;
            startPoint = currentPoint = e.GetPosition(canvasMap);
            if (mapMenu != null && canvasMap.Children.Contains(mapMenu))
            {
                canvasMap.Children.Remove(mapMenu);
            }
        }

        private void imageBack_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                textBoxX.Text = (x = Convert.ToInt32((currentX + canvasMap.Clip.Bounds.Width / 2) / VillageOnMap.Size)).ToString();
                textBoxY.Text = (y = Convert.ToInt32((currentY + canvasMap.Clip.Bounds.Height / 2) / VillageOnMap.Size)).ToString();
                moved = true;
                Point nextPoint = e.GetPosition(canvasMap);
                Point delta = new Point(nextPoint.X - currentPoint.X, nextPoint.Y - currentPoint.Y);
                MoveAll(delta.X, delta.Y);
                previousPoint = currentPoint;
                currentPoint = nextPoint;
                HideInvisible();
                ShowVisible();
            }
        }
        private void HideInvisible()
        {
            for (int i = canvasFirstIcon; i < canvasMap.Children.Count; i++)
            {
                VillageOnMap villageOnMap = canvasMap.Children[i] as VillageOnMap;
                if (Math.Abs(villageOnMap.x - x) > 23 || Math.Abs(villageOnMap.y - y) > 16)
                {
                    canvasMap.Children.Remove(villageOnMap);
                    i--;
                }
            }
        }
        private void ShowVisible()
        {
            foreach (VillageOnMap villageOnMap in villagesOnMap)
            {
                if (Math.Abs(villageOnMap.x - x) <= 23 && Math.Abs(villageOnMap.y - y) <= 16)
                {
                    if (!canvasMap.Children.Contains(villageOnMap))
                    {
                        canvasMap.Children.Add(villageOnMap);
                        villageOnMap.SetCoords();
                    }
                }
            }
        }

        private void imageBack_MouseLeave(object sender, MouseEventArgs e)
        {
            previousPoint = currentPoint;
            imageBack_MouseUp(sender, e);
        }

        private void imageBack_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                //currentX = (x + 0.5) * VillageOnMap.Size - canvasMap.Clip.Bounds.Width / 2;
                //currentY = (y + 0.5) * VillageOnMap.Size - canvasMap.Clip.Bounds.Height / 2;
                dragging = false;

                //if ((currentPoint - startPoint).Length > 20)
                //{
                //    Vector speed = currentPoint - previousPoint;
                //    int n = speed.Length > 5 ? Convert.ToInt32(speed.Length) * 8 : 0;
                //    Vector delta = speed / 5;
                //    for (int i = 0; !dragging && i < n; i++)
                //    {
                //        MoveAll(delta.X, delta.Y);
                //        delta *= 0.98;
                //        await Task.Delay(3);
                //    }
                //}
            }
        }

        void MoveAll(double x, double y)
        {
            currentX -= x;
            currentY -= y;


            for (int i = canvasFirstIcon; i < canvasMap.Children.Count; i++)
            {
                VillageOnMap villageOnMap = canvasMap.Children[i] as VillageOnMap;
                Canvas.SetLeft(villageOnMap, Canvas.GetLeft(villageOnMap) + x);
                Canvas.SetTop(villageOnMap, Canvas.GetTop(villageOnMap) + y);
            }
        }

        private void buttonToggle_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxX.IsVisible)
            {
                textBoxX.Visibility = Visibility.Collapsed;
                textBoxY.Visibility = Visibility.Collapsed;
                buttonGo.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBoxX.Visibility = Visibility.Visible;
                textBoxY.Visibility = Visibility.Visible;
                buttonGo.Visibility = Visibility.Visible;
            }
        }

        private void buttonGo_Click(object sender, RoutedEventArgs e)
        {
            MoveAllTo(Convert.ToInt32(textBoxX.Text), Convert.ToInt32(textBoxY.Text));
        }

        void MoveAllTo(double x, double y)
        {
            this.x = Convert.ToInt32(x);
            this.y = Convert.ToInt32(y);

            currentX = (x + 0.5) * VillageOnMap.Size - canvasMap.Clip.Bounds.Width / 2;
            currentY = (y + 0.5) * VillageOnMap.Size - canvasMap.Clip.Bounds.Height / 2;

            canvasMap.Children.RemoveRange(canvasFirstIcon, canvasMap.Children.Count - canvasFirstIcon);
            ShowVisible();
        }
    }
}
