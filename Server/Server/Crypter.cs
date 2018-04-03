using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CryptoPro.Sharpei;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.IO;
using System.Text.RegularExpressions;
using Kzar.ASN1.BER;

namespace Server
{
    class Crypter
    {
        private Gost3411CryptoServiceProvider hasher = new Gost3411CryptoServiceProvider();
        private Gost28147CryptoServiceProvider encrypter = new Gost28147CryptoServiceProvider();

        public byte[] hash = null;
        public byte[] signature = null;
        public byte[] sCert = null;
        public DateTime sTime;
        public byte[] hMsg = null;

        public byte[] eMsg = null;

        public X509Certificate2Collection Certificates { get; } = new X509Certificate2Collection();
        SecureString pswd = new SecureString();

        public Crypter()
        {
            // set certificates
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            foreach (var cert in store.Certificates)
            {
                if (Regex.IsMatch(cert.FriendlyName, @"test"))
                    Certificates.Add(cert);
            }

        }

        public void Encrypt(byte[] msg, int certIndex)
        {
            if (certIndex < 0 || certIndex > Certificates.Count)
                throw new Exception("Некорретктный индекс сертификата");
            EnvelopedCms cms = new EnvelopedCms(new ContentInfo(msg), new AlgorithmIdentifier(new Oid("1.2.643.2.2.21")));
            CmsRecipient recip = new CmsRecipient(SubjectIdentifierType.IssuerAndSerialNumber, Certificates[certIndex]);
            cms.Encrypt(recip);
            eMsg = cms.Encode();
            string aa = "";
            foreach (var item in cms.ContentInfo.Content)
                aa += item.ToString("x") + ":";

        }
        public byte[] CreateEMsg()
        {
            return eMsg;
        }
        public void ComputeHash(byte[] msg)
        {
            hash = hasher.ComputeHash(msg);
            hMsg = msg;
        }
        public void Sign(int certIndex)
        {
            if (certIndex < 0 || certIndex > Certificates.Count || hash == null)
                throw new Exception("Некорректный индекс сертификата или хэш равен нулю");

            var gost = (Gost3410CryptoServiceProvider)Certificates[certIndex].PrivateKey;
            if (gost == null)
                throw new Exception("У сертификата нет приватного ключа");
            gost.SetContainerPassword(pswd);
            signature = gost.CreateSignature(hash);
            sCert = Certificates[certIndex].Export(X509ContentType.Cert);
            sTime = DateTime.UtcNow;
        }
        public byte[] CreateSignature()
        {
            //...
            BERelement mainSeq = new BERelement(0x30);
            //...
            BERelement signSeq = new BERelement(0x30);
            signSeq.AddItem(new BERelement(0x0c, Encoding.UTF8.GetBytes("sign")));
            signSeq.AddItem(new BERelement(0x02, signature));
            signSeq.AddItem(new BERelement(0x02, sCert));
            signSeq.AddItem(new BERelement(0x0c, Encoding.UTF8.GetBytes(sTime.ToString())));
            //...
            BERelement fileSeq = new BERelement(0x30);
            fileSeq.AddItem(new BERelement(0x02, BitConverter.GetBytes(hMsg.Length)));
            //...
            mainSeq.AddItem(signSeq);
            mainSeq.AddItem(fileSeq);
            var aa = mainSeq.GetEncodedPacket();
            return mainSeq.GetEncodedPacket().Concat(hMsg).ToArray();
        }
    }
}
