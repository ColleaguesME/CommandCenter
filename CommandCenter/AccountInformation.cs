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
    public class AccountInformation
    {
        static int lastUnusedPort = 8000;
        const string cookiesPath = "";
        public static string myIp;
        byte[] buffer;

        string name, password;
        TcpListener tcpListener;
        public bool isClosed;
        public Socket socket;
        public MainWindow mainWindow;

        public AccountInformation(string source, MainWindow mainWindow)
        {
            buffer = new byte[65536];
            isClosed = true;
            this.mainWindow = mainWindow;
            mainWindow.accounts.Add(this);
            string[] parameters = source.Split(' ');
            name = parameters[0];
            password = parameters[0];
            tcpListener = new TcpListener(IPAddress.Any, lastUnusedPort);
            tcpListener.Start();
            tcpListener.BeginAcceptSocket(EndAcceptSocket, null);
            Process process = new Process();
            process.StartInfo.FileName = "node";
            process.StartInfo.Arguments = "..\\..\\..\\makeConnection.js " + name + " " + password + " " + myIp + " " + lastUnusedPort.ToString();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.EnableRaisingEvents = true;
            Action<Task<string>> callback = null;
            callback = result =>
            {
                mainWindow.Dispatcher.Invoke(() =>
                {
                    if (result.IsCompleted)
                    {
                        mainWindow.textBox.Text += result.Result + "\n";
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
            socket = tcpListener.EndAcceptSocket(argument);
            isClosed = false;
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, GetVillage, null);
        }
        void GetVillage(IAsyncResult argument)
        {
            mainWindow.Dispatcher.Invoke(() =>
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Village));
                int recieved = socket.EndReceive(argument);
                if (recieved == 0)
                {
                    int oldPort = (tcpListener.LocalEndpoint as IPEndPoint).Port;
                    isClosed = true;
                    socket.Close();
                    tcpListener.Stop();
                    tcpListener = new TcpListener(IPAddress.Any, oldPort);
                    tcpListener.Start();
                    tcpListener.BeginAcceptSocket(EndAcceptSocket, null);
                }
                else
                {
                    string str = Encoding.UTF8.GetString(buffer);
                    MemoryStream stream = new MemoryStream(buffer, 0, recieved);
                    Village recievedVillage = (Village)serializer.ReadObject(stream);
                    //Village recievedVillage = Serializer.Get().Deserialize<Village>(str);
                    bool found = false;
                    foreach (Village village in mainWindow.villages)
                    {
                        if (village.x == recievedVillage.x && village.y == recievedVillage.y)
                        {
                            found = true;
                            village.Update(recievedVillage);
                        }
                    }
                    if (!found)
                    {
                        Village village = new Village(recievedVillage, this);
                        village.Bar.MouseDoubleClick += mainWindow.OpenTab;
                        (mainWindow.stackPanelKach as IAddChild).AddChild(village.Bar);
                        mainWindow.villages.Add(village);
                    }
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, GetVillage, null);
                }
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
