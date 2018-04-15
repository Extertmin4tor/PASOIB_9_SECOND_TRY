using System;
using CryptoPro.Sharpei;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;


namespace Client
{
    class Crypter
    {

        private X509Certificate2 serverCert = new X509Certificate2();
        private Gost28147 symKey;
        public byte[] IV { get; set; }
        private Gost3411CryptoServiceProvider hasher = new Gost3411CryptoServiceProvider();
        private byte[] hData;
        public X509Certificate2Collection ClientCertificates { get; } = new X509Certificate2Collection();
        private byte[] signature = null;
        private byte[] sCert = null;
        private DateTime sTime;
        private byte[] hash;

        public Crypter()
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            foreach (var cert in store.Certificates)
            {
                if (Regex.IsMatch(cert.FriendlyName, @"test"))
                    ClientCertificates.Add(cert);
            }
        }
        

        public void ComputeHash(byte[] data)
        {
            hash = hasher.ComputeHash(data);
            hData = data;
            
        }


        public byte[] GetHashValue()
        {
                return hasher.Hash;
        }


        public void FromBytesToCert(byte[] data)
        {
            try
            {
                serverCert.Import(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public byte[] GetEncryptedSymKey()
        {
            try
            {
                symKey = new Gost28147CryptoServiceProvider();
                IV = symKey.IV;
                Gost2012_256KeyExchangeFormatter Formatter = new Gost2012_256KeyExchangeFormatter(serverCert.PublicKey.Key);
                GostKeyTransport encKey = Formatter.CreateKeyExchange(symKey);
                byte[] bencKey = encKey.Encode();
                return bencKey;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }



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

        public byte[] Sign(byte[] data, int certIndex)
        {
            try
            {
                if(hash == null)
                {
                    throw new NullReferenceException("Compute hash first");
                }
                CspParameters cp = new CspParameters();
                var privateKey = ClientCertificates[certIndex].PrivateKey as Gost3410_2012_256CryptoServiceProvider;
                var uniqueKeyContainerName = privateKey.CspKeyContainerInfo.UniqueKeyContainerName;
                cp.KeyContainerName = uniqueKeyContainerName;
                cp.ProviderType = 75;
                cp.ProviderName = null;
                Gost3410_2012_256 gkey = new Gost3410_2012_256CryptoServiceProvider(cp);
                Gost3410_2012_256CryptoServiceProvider srcContainer = new Gost3410_2012_256CryptoServiceProvider(cp);
                Gost3410Parameters srcPublicKeyParameters = srcContainer.ExportParameters(false);
                if (srcContainer == null)
                    throw new Exception("У сертификата нет приватного ключа");
                signature = srcContainer.CreateSignature(hasher.Hash);
                sCert = ClientCertificates[certIndex].Export(X509ContentType.Cert);
                sTime = DateTime.Now;
                return Asn1Formatter.CreateSignature(signature, sCert, sTime, data);
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

       


        public byte[] Encrypt(byte[] data)
        {
            int currentPosition = 0;
            byte[] targetBytes = new byte[1024];
            int sourceByteLength = data.Length;
            CPCryptoAPITransform cryptoTransform = (CPCryptoAPITransform)symKey.CreateEncryptor();
            int inputBlockSize = cryptoTransform.InputBlockSize;
            int outputBlockSize = cryptoTransform.OutputBlockSize;
            try
            {
                if (cryptoTransform.CanTransformMultipleBlocks)
                {
                    int numBytesRead = 0;
                    while (sourceByteLength - currentPosition >= inputBlockSize)
                    {
                        numBytesRead = cryptoTransform.TransformBlock(
                        data, currentPosition,
                        inputBlockSize, targetBytes,
                            currentPosition);
                        currentPosition += numBytesRead;
                    }
                    // Преобразуем последний блок.
                    byte[] finalBytes = cryptoTransform.TransformFinalBlock(
                        data, currentPosition,
                        sourceByteLength - currentPosition);

                    // Записываем последний зашифрованный блок 
                    // в массив targetBytes.
                    finalBytes.CopyTo(targetBytes, currentPosition);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Caught unexpected exception:" + ex.ToString());
            }
            // Определяем, может ли CPCryptoAPITransform использоваться повторно.
            if (!cryptoTransform.CanReuseTransform)
            {
                // Освобождаем занятые ресурсы.
                cryptoTransform.Clear();
            }
            // Убираем неиспользуемые байты из массива.
            return TrimArray(targetBytes);
        }

    }
}
