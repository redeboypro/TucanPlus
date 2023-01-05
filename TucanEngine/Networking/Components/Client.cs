using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK;
using TucanEngine.Main.GameLogic.Common;

namespace TucanEngine.Networking.Components
{
    public class Client : Behaviour, INetworkComponent
    {
        public ReceiveDataEvent ReceiveData { get; set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }

        private bool isSerialized = true;
        private TcpClient clientSocket;
        private NetworkStream networkStream;
        private Thread receiveThread;

        public void Connect(string ip, int port) {
            Ip = ip;
            Port = port;

            clientSocket = new TcpClient();
            clientSocket.Connect(ip, port);
            
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
            networkStream.Read(serializedData, 0, serializedData.Length);
            var dataFromClient = Package.Deserialize(Encoding.ASCII.GetString(serializedData));
            networkStream.Flush();
            ReceiveData?.Invoke(dataFromClient);
            isSerialized = true;
        }
        
        public override void OnUpdateFrame(FrameEventArgs e) {
            if (clientSocket is null) {
                return;
            }

            try {
                networkStream = clientSocket.GetStream();
                if (networkStream.DataAvailable) {
                    if (isSerialized) {
                        isSerialized = false;
                        receiveThread.Start();
                    }
                }
            }
            catch {
                Console.WriteLine("Unable to receive server data");
            }
        }

        public void SendDataToOther(string serializedData) {
            try {
                var serverStream = clientSocket.GetStream();
                var outStream = Encoding.ASCII.GetBytes(serializedData);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
            }
            catch {
                Console.WriteLine("Unable to send client data");
            }
        }
    }
}