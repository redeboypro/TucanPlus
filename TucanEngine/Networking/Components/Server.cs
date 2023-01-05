using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using TucanEngine.Common.Serialization;
using TucanEngine.Main.GameLogic.Common;

namespace TucanEngine.Networking.Components {
    public class Server : Behaviour, INetworkComponent
    {
        public static float Latency { get; set; } = 0.15f;
        
        public ReceiveDataEvent ReceiveData { get; set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }

        private bool isSerialized = true;
        private TcpListener serverSocket;
        private TcpClient clientSocket;
        private NetworkStream networkStream;
        private Thread receiveThread;
        private float countDown;

        public void Start(string ip, int port) {
            Ip = ip;
            Port = port;
            countDown = Latency;
            
            var ipAddress = IPAddress.Parse(ip);
            serverSocket = new TcpListener(ipAddress, port);
            serverSocket.Start();
            
            Console.WriteLine("Server started");
            
            clientSocket = serverSocket.AcceptTcpClient();
            
            var evt = new AutoResetEvent(true);
            receiveThread = new Thread(() => {
                evt.WaitOne();
                while (receiveThread.ThreadState == ThreadState.Running) {
                    OnReceiveData();
                }
            });
        }

        private void OnReceiveData() {
            var serializedData = new byte[4096];
            networkStream.Read(serializedData, 0, (int)serializedData.Length);
            var dataFromClient = Package.Deserialize(Encoding.ASCII.GetString(serializedData));
            networkStream.Flush();
            ReceiveData?.Invoke(dataFromClient);
            dataFromClient.Clear();
            isSerialized = true;
        }

        public override void OnUpdateFrame(FrameEventArgs e) {
            if (clientSocket is null || serverSocket is null) {
                return;
            }

            if (countDown > 0) {
                countDown -= (float)e.Time;
                return;
            }

            countDown = Latency;

            try {
                networkStream = clientSocket.GetStream();
                if (networkStream.DataAvailable) {
                    if (isSerialized) {
                        isSerialized = false;
                        receiveThread.Start();
                    }
                }
            }
            catch (Exception exception) {
                Console.WriteLine("Unable to receive server data" + exception);
            }
        }

        public void SendDataToOther(string serializedData) {
            try {
                var clientStream = clientSocket.GetStream();
                var outStream = Encoding.ASCII.GetBytes(serializedData);
                clientStream.Write(outStream, 0, outStream.Length);
                clientStream.Flush();
            }
            catch (Exception e) {
                Console.WriteLine("Unable to send server data");
            }
        }
    }
}