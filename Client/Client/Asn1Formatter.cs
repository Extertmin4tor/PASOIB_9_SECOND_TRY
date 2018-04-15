using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kzar.ASN1.BER;

namespace Client
{
    class Asn1Formatter
    {
        static public byte[] SetCommandAsn1(int cmd)
        {
            BERelement asn = new BERelement(0x30);
            asn.AddItem(new BERelement(0x02, cmd));
            return asn.GetEncodedPacket();
        }

        static public byte[] SetCertNameAsn1(byte[] certName)
        {
            BERelement asn = new BERelement(0x30);
            asn.AddItem(new BERelement(0x04, certName));
            return asn.GetEncodedPacket();
        }

        static public byte[] GetCertAsn1(byte[] data)
        {
            var asn = BERelement.DecodePacket(data);
            byte[] cert = asn.Items[0].Value;
            return cert;
        }

        static public byte[] SetSymKeyAndIVAsn1(byte[] data, byte[] iv)
        {
            BERelement asn = new BERelement(0x30);
            asn.AddItem(new BERelement(0x04, data));
            asn.AddItem(new BERelement(0x04, iv));
            return asn.GetEncodedPacket();

        }

        static public byte[] CreateSignature( byte[] signature, byte[] sCert, DateTime sTime,  byte[] data)
        {
            BERelement mainSeq = new BERelement(0x30);
            BERelement signSeq = new BERelement(0x30);
            signSeq.AddItem(new BERelement(0x0C, signature));
            signSeq.AddItem(new BERelement(0x0C, sCert));
            signSeq.AddItem(new BERelement(0x0c, BitConverter.GetBytes(sTime.ToBinary())));
            BERelement fileSeq = new BERelement(0x30);
            fileSeq.AddItem(new BERelement(0x0C, data));
            mainSeq.AddItem(signSeq);
            mainSeq.AddItem(fileSeq);
            return mainSeq.GetEncodedPacket();
        }
    }
}
