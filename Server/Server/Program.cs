using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Kzar.ASN1.BER;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    class Program
    {
        public enum Cmd { certs, cipher, hash, sign };
        private static Crypter crypter  = new Crypter();
        private static ServerSocket socket = new ServerSocket();

        static void Main(string[] args)
        {

            try
            {
                socket.Init();
                byte[] data = socket.Recieve();
                BERelement asn = BERelement.DecodePacket(data);
                int operation;
                if (asn.Items[0].Value.Length > 1)
                {
                    operation = BitConverter.ToInt32(asn.Items[0].Value, 0);
                }
                else
                {
                    operation = asn.Items[0].Value[0];
                }
                
                switch (operation)
                {
                    case (int)Cmd.certs:
                        SendCertificatesList();
                        break;
                    case (int)Cmd.cipher:
                        break;
                    case (int)Cmd.hash:
                        break;
                    case (int)Cmd.sign:
                        break;
                    default:
                        break;


                }

               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private static void SendCertificatesList()
        {
            var certs = crypter.Certificates;
            BERelement asnCerts = new BERelement(0x30);

            foreach(var name in certs)
            {
                asnCerts.AddItem(new BERelement(0x0C, name.FriendlyName));
            }

            socket.Send(asnCerts.GetEncodedPacket());
           
        }


    }
}