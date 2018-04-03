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
        private int port = 8005; // порт сервера
        private string address = "127.0.0.1"; // адрес сервера
        private IPEndPoint ipEndPoint = null;
        private IPAddress ipAddr = null;
        public string IPString { get { return address; } }
        public int PortString { get { return port; } }
        Socket socket;
        Socket acceptSocket;


        public ServerSocket(string _address, int _port)
        {
            address = _address;
            port = _port;
            ipAddr = IPAddress.Parse(address);
            ipEndPoint = new IPEndPoint(ipAddr, port);
        }

        public ServerSocket()
        {
            ipAddr = IPAddress.Parse(address);
            ipEndPoint = new IPEndPoint(ipAddr, port);
        }
        public void Init()
        {
            socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            socket.Bind(ipEndPoint);
            socket.Listen(10);
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
        }
        public void Send(byte[] data)
        {
            int bytesSent = acceptSocket.Send(data);
        }

        public byte[] Recieve()
        {
            acceptSocket = socket.Accept();

            int bytesCount = 0;
            byte[] data = new byte[1024];
            bytesCount = acceptSocket.Receive(data);
            byte[] normalizeData = new byte[bytesCount];
            Array.Copy(data, normalizeData, bytesCount);

            return normalizeData;
        }

        public void Refuse()
        {
            acceptSocket.Shutdown(SocketShutdown.Both);
            acceptSocket.Close();
        }
    }
}
