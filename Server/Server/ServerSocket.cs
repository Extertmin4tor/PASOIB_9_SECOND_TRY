using Kzar.ASN1.BER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerSocket
    {
        private int port = 8175; // порт сервера
        private string address = "127.0.0.1"; // адрес сервера
        private IPEndPoint ipEndPoint = null;
        private IPAddress ipAddr = null;
        public string IPString { get { return address; } }
        public int PortString { get { return port; } }
        Socket socket;
        Socket acceptSocket;
        private static bool flag = false;


        public ServerSocket(string _address, int _port)
        {
            address = _address;
            port = _port;
            ipAddr = IPAddress.Parse(address);
            ipEndPoint = new IPEndPoint(ipAddr, port);
        }

        public ServerSocket()
        {

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = ip.ToString();
                }
            }
            ipAddr = IPAddress.Parse(address);
            ipEndPoint = new IPEndPoint(ipAddr, port);
        }
        public void Init()
        {
            socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ipEndPoint);
            socket.Listen(10);
            Console.WriteLine("Server have been started. Waiting for connections...");
        }
        public void Send(byte[] data)
        {
            int bytesSend = acceptSocket.Send(data);
            Console.WriteLine("Bytes sended: " + bytesSend);
        }

        public byte[] Recieve()
        {
            if (!flag)
            {
                acceptSocket = socket.Accept();
                flag = true;
            }


            int bytesCount = 0;
            byte[] data = new byte[1024];
            bytesCount = acceptSocket.Receive(data);
            Console.WriteLine("Bytes recieved: " + bytesCount);
            byte[] normalizeData = new byte[bytesCount];
            Array.Copy(data, normalizeData, bytesCount);

            return normalizeData;
        }

        public void Refuse()
        {
            acceptSocket.Shutdown(SocketShutdown.Both);
            acceptSocket.Close();
            flag = false;
        }
    }
}
