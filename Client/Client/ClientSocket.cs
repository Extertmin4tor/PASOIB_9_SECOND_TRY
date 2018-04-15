using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Client
{
    class ClientSocket
    {
        private int port = 8005; // порт сервера
        private string address = "127.0.0.1"; // адрес сервера
        private IPEndPoint ipEndPoint = null;
        private IPAddress ipAddr = null;
        public string IPString { get { return address; } }
        public int PortString { get { return port; } }
        Socket sender;
        static public Object lockObject;


        public ClientSocket(string _address, int _port)
        {
            address = _address;
            port = _port;
            ipAddr = IPAddress.Parse(address);
            ipEndPoint = new IPEndPoint(ipAddr, port);
        }

        public ClientSocket()
        {
            ipAddr = IPAddress.Parse(address);
            ipEndPoint = new IPEndPoint(ipAddr, port);
        }


        public void Init()
        {
            sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipEndPoint);
        }


        public void Send(byte[] data)
        {
            int bytesSent = sender.Send(data);
        }

        public byte[] Recieve()
        {

            int bytesCount = 0;
            byte[] data = new byte[1024];
            bytesCount = sender.Receive(data);
            byte[] normalizeData = new byte[bytesCount];
            Array.Copy(data, normalizeData, bytesCount);

            return normalizeData;

        }

        public void Refuse()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
