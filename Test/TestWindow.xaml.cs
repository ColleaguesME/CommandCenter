
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MySql;
using MySql.Data;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using MySql.Fabric;
using MySql.Web;

namespace Test
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class TestWindow : System.Windows.Window
    {
        // insert/update/delete
        //MySqlCommand msc = new MySqlCommand();
        //msc.Connection = mySqlConnection;
        //msc.CommandText = string.Format(@"insert into villageinfos values(500, 500, ""{0}"", ""village name"", ""province name"", ""player name"", ""tribe name (tn )"", 10);", (new DateTime(2016, 8, 1)).ToString(new CultureInfo("ja-JP")));
        //msc.ExecuteNonQuery();

        // select
        //MySqlCommand mySqlCommand = new MySqlCommand("SELECT * FROM villageinfos", mySqlConnection);
        //mySqlDataReader = mySqlCommand.ExecuteReader();
        //while (mySqlDataReader.Read())
        //{
        //    textBox.Text += mySqlDataReader.GetInt32(0) + " | " + 
        //        mySqlDataReader.GetInt32(1) + " | " +
        //        mySqlDataReader.GetDateTime(2) + " | " +
        //        mySqlDataReader.GetString(3) + " | " +
        //        mySqlDataReader.GetString(4) + " | " +
        //        mySqlDataReader.GetString(5) + " | " +
        //        mySqlDataReader.GetString(6) + " | " +
        //        mySqlDataReader.GetInt32(7) + '\n';
        //}

        DataContractJsonSerializer serializer;
        MySqlConnection mySqlConnection;
        MySqlDataReader mySqlDataReader;
        public TestWindow()
        {
            InitializeComponent();

            serializer = new DataContractJsonSerializer(typeof(VillageInfo));
            //mySqlConnection = new MySqlConnection("server=127.0.0.1;uid=root;pwd=pavellev7;database=db;");
            //mySqlConnection.Open();
            are = new AutoResetEvent(false);
        }
        

        [DataContract]
        class VillageInfo
        {
            [DataMember]
            string name, tribe, player, province;
            [DataMember]
            int points;
            int x, y;
        }
        AutoResetEvent are;
        List<VillageInfo> villages;
        string buffer;
        private void Test(object sender, System.Windows.RoutedEventArgs e)
        {

            List<int> pointsTable = new List<int>()
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
            int index = pointsTable.BinarySearch(500);
            if (index < 0)
            {
                index = -index - 2;
            }
            textBox.Text += index.ToString() + " | " +  Convert.ToChar(66 + index);
        }

        private void GetInfo(object sender, System.Windows.RoutedEventArgs e)
        {

            Process process = new Process();
            process.StartInfo.FileName = "node";
            process.StartInfo.Arguments = "..\\..\\..\\..\\CommandCenter\\makeConnection.js";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;
            Action<Task<string>> callback = null;
            callback = result =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (result.Result != "")
                    {
                        textBox.Text += result.Result + "\n";
                    }
                });
                Thread.Sleep(100);
                process.StandardOutput.ReadLineAsync().ContinueWith(callback);
            };
            process.Start();
            process.StandardOutput.ReadLineAsync().ContinueWith(callback);

        }
       

        private void EnterGame(object sender, System.Windows.RoutedEventArgs e)
        {

            buttonEnterGame.IsEnabled = false;
        }
        

        private void Window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
