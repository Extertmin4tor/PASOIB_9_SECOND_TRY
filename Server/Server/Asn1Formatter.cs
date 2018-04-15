using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kzar.ASN1.BER;
using System.Security.Cryptography.X509Certificates;

namespace Server
{
    public class Asn1Formatter
    {
        public static byte[] SetCertASN1(byte[] cert)
        {
            var mainSeq = new BERelement(0x30);
            mainSeq.AddItem(new BERelement(0x0C, cert));
            return mainSeq.GetEncodedPacket();

        }

        public static byte[] SetCertListASN1(X509Certificate2Collection certs)
        {
            var asnCerts = new BERelement(0x30);

            foreach (var name in certs)
            {
                asnCerts.AddItem(new BERelement(0x0C, name.FriendlyName));
            }

            return asnCerts.GetEncodedPacket();

        }

        static public byte[] GetSymAsn1(byte[] data)
        {
            var asn = BERelement.DecodePacket(data);
            byte[] symKey = asn.Items[0].Value;
            return symKey;
        }

        static public byte[] GetIVAsn1(byte[] data)
        {
            var asn = BERelement.DecodePacket(data);
            byte[] iv = asn.Items[1].Value;
            return iv;
        }
    }
}
