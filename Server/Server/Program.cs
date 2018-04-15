using System;
using System.Text;
using Kzar.ASN1.BER;
using System.Security.Cryptography.X509Certificates;
using CryptoPro.Sharpei;
using System.Linq;


namespace Server
{
    class Program
    {
        public enum Cmd { certs, cipher, sign, error };
        private static Crypter crypter  = new Crypter();
        private static ServerSocket socket = new ServerSocket();

        static void Main(string[] args)
        {

            try
            {
                socket.Init();
                while (true)
                {
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
                            SendCertificatesList();
                            data = socket.Recieve();
                            asn = BERelement.DecodePacket(data);
                            string certName = Encoding.ASCII.GetString(asn.Items[0].Value);
                            foreach(var cert in crypter.Certificates)
                            {
                                if(cert.FriendlyName == certName)
                                {
                                    crypter.currentCertificate = cert;
                                    SendPublicKey(cert);
                                    break;
                                }
                            }

                            data = socket.Recieve();
                            crypter.SetSymmetrKey(Asn1Formatter.GetSymAsn1(data));
                            crypter.IV = Asn1Formatter.GetIVAsn1(data);
                            byte[] asn1Established = Asn1Formatter.SetCertASN1(Encoding.ASCII.GetBytes("ESTABLISHED"));
                            socket.Send(asn1Established);
                            data = socket.Recieve();
                            String text = Encoding.ASCII.GetString(crypter.Decrypt(data));
                            Console.WriteLine("Recieved data from client: " + text);
                            break;
                        case (int)Cmd.sign:
                            asn1Established = Asn1Formatter.SetCertASN1(Encoding.ASCII.GetBytes("ESTABLISHED"));
                            socket.Send(asn1Established);
                            data = socket.Recieve();
                            asn = BERelement.DecodePacket(data);
                            try
                            {
                                if (asn.Items[0].Value.Length > 1)
                                {
                                    operation = BitConverter.ToInt32(asn.Items[0].Value, 0);
                                }
                                else
                                {
                                    operation = asn.Items[0].Value[0];
                                }
                                if (operation == (int)Cmd.error)
                                {
                                    Console.WriteLine("Error!");
                                    continue;
                                }
                            }
                            catch(NotSupportedException)
                            {
                                Console.WriteLine("Signed message came!");
                            }

                            BERelement mSeq = BERelement.DecodePacket(data);
                            BERelement sSeq = mSeq.Items[0];
                            BERelement fSeq = mSeq.Items[1];

                            var signature = sSeq.Items[0].Value;
                            var certS = new X509Certificate2(sSeq.Items[1].Value);
                            var time = DateTime.FromBinary(BitConverter.ToInt64(sSeq.Items[2].Value, 0));
                            var sData = fSeq.Items[0].Value;

                            Gost3410_2012_256CryptoServiceProvider sGost = (Gost3410_2012_256CryptoServiceProvider)certS.PublicKey.Key;
                            Gost3411CryptoServiceProvider hGost = new Gost3411CryptoServiceProvider();
                            bool correct = sGost.VerifySignature(hGost.ComputeHash(sData), signature);

                            Console.WriteLine($"Friendly name: {certS.FriendlyName}");
                            Console.WriteLine(certS + "\n");
                            Console.WriteLine("Signature time...");
                            Console.WriteLine(time + "\n");
                            Console.WriteLine("Correct signature?...");
                            Console.WriteLine(correct + "\n");
                            Console.WriteLine("Data:");
                            Console.WriteLine(Encoding.ASCII.GetString(sData) + "\n");
                            break;
                        default:
                            break;


                    }
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
            byte[] data = Asn1Formatter.SetCertListASN1(certs);
            socket.Send(data);
           
        }

        private static void SendPublicKey(X509Certificate2 cert)
        {
            var certBytes = cert.Export(X509ContentType.Cert);
            byte[] asn1PubKey = Asn1Formatter.SetCertASN1(certBytes);
            socket.Send(asn1PubKey);
        }

    }
}