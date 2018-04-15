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
    
        public X509Certificate2Collection Certificates { get; } = new X509Certificate2Collection();
        private Gost28147 symKey = new Gost28147CryptoServiceProvider();
        public X509Certificate2 currentCertificate { get; set; }
        public byte[] IV { get; set; }


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
        public byte[] Decrypt(byte[] data)
        {
            symKey.IV = IV;
            byte[] targetBytes = new byte[1024];
            int currentPosition = 0;

            // Создаем дешифратор для ГОСТ.
            CPCryptoAPITransform cryptoTransform =
                (CPCryptoAPITransform)symKey.CreateDecryptor();

            int inputBlockSize = cryptoTransform.InputBlockSize;
            int sourceByteLength = data.Length;

            try
            {
                int numBytesRead = 0;
                while (sourceByteLength - currentPosition >= inputBlockSize)
                {
                    // Преобразуем байты начиная с currentPosition в массиве 
                    // sourceBytes, записывая результат в массив targetBytes.
                    numBytesRead = cryptoTransform.TransformBlock(
                        data,
                        currentPosition,
                        inputBlockSize,
                        targetBytes,
                        currentPosition);

                    currentPosition += numBytesRead;
                }

                // Преобразуем последний блок.
                byte[] finalBytes = cryptoTransform.TransformFinalBlock(
                    data,
                    currentPosition,
                    sourceByteLength - currentPosition);

                // Записываем последний расшифрованный блок 
                // в массив targetBytes.
                finalBytes.CopyTo(targetBytes, currentPosition);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught unexpected exception:" + ex.ToString());
            }
            // Убираем неиспользуемые байты из массива.
            return TrimArray(targetBytes);
        }

        private static byte[] TrimArray(byte[] targetArray)
        {
            var enum1 = targetArray.GetEnumerator();
            int i = 0;
            while (enum1.MoveNext())
            {
                if (enum1.Current.ToString().Equals("0"))
                {
                    break;
                }
                i++;
            }
            // Создаем новый массив нужного размера.
            byte[] returnedArray = new byte[i];
            for (int j = 0; j < i; j++)
            {
                returnedArray[j] = targetArray[j];
            }
            return returnedArray;
        }


        public void SetSymmetrKey(byte[] data)
        {
            Gost2012_256KeyExchangeDeformatter deformatter = new Gost2012_256KeyExchangeDeformatter(currentCertificate.PrivateKey);
            GostKeyTransport encKey = new GostKeyTransport();
            encKey.Decode(data);
            symKey = deformatter.DecryptKeyExchange(encKey) as Gost28147;
        }

  
    }
}
