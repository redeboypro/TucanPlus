using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Networking
{
    internal class Program
    {
        public static void Main(string[] args) {
            var address = IPAddress.Parse("26.250.248.132");
            var serverSocket = new TcpListener(address, 7777);
            var clientSocket = default(TcpClient);
            serverSocket.Start();
            Console.WriteLine("Server started");
            clientSocket = serverSocket.AcceptTcpClient();
            while (true) {
                try {
                    var networkStream = clientSocket.GetStream();
                    var serializedData = new byte[10025];
                    if (networkStream.DataAvailable) {
                        networkStream.Read(serializedData, 0, (int)serializedData.Length);
                        var dataFromClient = Encoding.ASCII.GetString(serializedData);
                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf('\0'));
                        Console.WriteLine("Data from client - " + dataFromClient);
                        networkStream.Flush();
                    }
                    var outStream = Encoding.ASCII.GetBytes(Console.ReadLine() ?? string.Empty);
                    clientSocket.GetStream().Write(outStream, 0, outStream.Length);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
    }
}