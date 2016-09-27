using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace CommandCenter
{
    [DataContract]
    public class AccountInformation
    {
        [DataMember]
        public List<Village> villages;
        [DataMember]
        public List<QuestReward> questRewards;
        [DataMember]
        public int smallRewardQuantity;

        
        static int lastUnusedPort = 8000;
        const string cookiesPath = "";
        public static string myIp;
        byte[] buffer;

        string name, password;
        TcpListener tcpListener;
        public bool isClosed;
        public NetworkStream networkStream;
        public List<byte> recievedBuffer;

        public DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AccountInformation));
        /// <summary>
        /// qwe
        /// </summary>
        /// <param name="source"></param>
        public AccountInformation(string source)
        {
            buffer = new byte[65536];
            isClosed = true;
            string[] parameters = source.Split(' ');
            name = parameters[0];
            password = parameters[0];
            tcpListener = new TcpListener(IPAddress.Any, lastUnusedPort);
            tcpListener.Start();
            tcpListener.BeginAcceptSocket(EndAcceptSocket, null);
            Process process = new Process();
            process.StartInfo.FileName = "node";
            process.StartInfo.Arguments = "..\\..\\..\\makeConnection.js " + lastUnusedPort.ToString() + "*" + myIp;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;
            Action<Task<string>> callback = null;
            callback = result =>
            {
                MainWindow.Current.Dispatcher.Invoke(() =>
                {
                    if (result.IsCompleted)
                    {
                        MainWindow.Current.textBox.Text += result.Result + "\n";
                    }
                });
                Thread.Sleep(100);
                process.StandardOutput.ReadLineAsync().ContinueWith(callback);
            };
            process.Start();
            process.StandardOutput.ReadLineAsync().ContinueWith(callback);
            lastUnusedPort++;
        }
        void EndAcceptSocket(IAsyncResult argument)
        {
            networkStream = new NetworkStream(tcpListener.EndAcceptSocket(argument));
            isClosed = false;
            networkStream.BeginRead(buffer, 0, buffer.Length, GetVillage, null);

        }
        void GetVillage(IAsyncResult argument)
        {
            MainWindow.Current.Dispatcher.Invoke(() =>
            {
                int recieved = networkStream.EndRead(argument);

                if (recieved == 0)
                {
                    int oldPort = (tcpListener.LocalEndpoint as IPEndPoint).Port;
                    isClosed = true;
                    networkStream.Close();
                    tcpListener.Stop();
                    MessageBox.Show("Closed connection");
                    tcpListener = new TcpListener(IPAddress.Any, oldPort);
                    tcpListener.Start();
                    tcpListener.BeginAcceptSocket(EndAcceptSocket, null);
                }
                else
                {
                    Enumerable.Concat(recievedBuffer, buffer);
                    int res = Array.IndexOf<byte>(buffer, 0x7d);
                    if (res != -1)
                    {
                        MemoryStream ms = new MemoryStream(recievedBuffer.GetRange(0, res + 1).ToArray());
                        Update((AccountInformation)serializer.ReadObject(networkStream));
                        recievedBuffer.RemoveRange(0, res + 1);
                    }
                    networkStream.BeginRead(buffer, 0, buffer.Length, GetVillage, null);
                 
                }
                    
            });
        }

        void Update(AccountInformation source)
        {
            questRewards = source.questRewards;
            smallRewardQuantity = source.smallRewardQuantity;

            source.villages.ForEach((recievedVillage) =>
            {
                try
                {
                    villages.First((village) =>
                    {
                        return village.x == recievedVillage.x && village.y == recievedVillage.y;
                    }).Update(recievedVillage);
                }
                catch
                {

                    Village village = new Village(recievedVillage, this);
                    MainWindow.Current.stackPanelKach.Children.Add(village.Bar);
                    villages.Add(village);
                    try
                    {
                        MainWindow.Current.villagesOnMap.First((villageOnMap) =>
                        {
                            return villageOnMap.x == recievedVillage.x && villageOnMap.y == recievedVillage.y;
                        }).village = village;
                    }
                    catch
                    {
                        //db updates once a day so we need to add manualy not found villages
                    }
                }
            });
        }
        public void ClaimReward(int index)
        {
            //rewrite
            questRewards.RemoveAt(index);
            villages.ForEach((village) =>
            {
                village.DisplayQuestRewards();
            });
        }

        public override string ToString()
        {
            return name + ' ' + password;
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
    }
}
