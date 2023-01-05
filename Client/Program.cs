using System;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        
        public static void Main(string[] args) {
            var clientSocket = new TcpClient();
            clientSocket.Connect("26.250.248.132", 7777);
            while (true) {
                var serverStream = clientSocket.GetStream();
                var outStream = Encoding.ASCII.GetBytes(Console.ReadLine() ?? string.Empty);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                var serializedData = new byte[10025];
                if (serverStream.DataAvailable) {
                    serverStream.Read(serializedData, 0, (int)serializedData.Length);
                    var dataFromServer = Encoding.ASCII.GetString(serializedData);
                    dataFromServer = dataFromServer.Substring(0, dataFromServer.IndexOf('\0'));
                    Console.WriteLine("Data from client - " + dataFromServer);
                    serverStream.Flush();
                }
            }
        }
    }
}