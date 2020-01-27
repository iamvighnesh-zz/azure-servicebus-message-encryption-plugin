using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.MessageEncryption.Providers
{
    public class RijndaelManagedCryptographyProvider : ICryptographyProvider
    {
        private readonly byte[] cryptoKeyBytes;

        private readonly byte[] initVectorKeyBytes;

        public RijndaelManagedCryptographyProvider(string cryptoKey, string initVerctorKey)
        {
            cryptoKeyBytes = CreateCryptographyKey(cryptoKey);

            initVectorKeyBytes = Encoding.UTF8.GetBytes(initVerctorKey);
        }

        public async Task<byte[]> Decrypt(byte[] input)
        {
            var decryptor = CreateDecryptor();

            var inputBytes = Convert.FromBase64String(Convert.ToBase64String(input));

            byte[] plainTextBytes = new byte[inputBytes.Length];

            using MemoryStream memoryStream = new MemoryStream(inputBytes);

            using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            await cryptoStream.ReadAsync(plainTextBytes, 0, plainTextBytes.Length);

            return plainTextBytes;
        }

        public async Task<byte[]> Encrypt(byte[] input)
        {
            var encryptor = CreateEncryptor();

            using MemoryStream memoryStream = new MemoryStream();
            
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            
            await cryptoStream.WriteAsync(input, 0, input.Length);
            
            cryptoStream.FlushFinalBlock();
            
            return memoryStream.ToArray();
        }

        private ICryptoTransform CreateEncryptor()
        {
            var password = new PasswordDeriveBytes(cryptoKeyBytes, null);
            
            var keyBytes = password.GetBytes(32);

            return GetAlgorithm().CreateEncryptor(keyBytes, initVectorKeyBytes);
        }

        private ICryptoTransform CreateDecryptor()
        {
            var password = new PasswordDeriveBytes(cryptoKeyBytes, null);

            var keyBytes = password.GetBytes(32);

            return GetAlgorithm().CreateDecryptor(keyBytes, initVectorKeyBytes);
        }

        private SymmetricAlgorithm GetAlgorithm()
        {
            return new RijndaelManaged { Mode = CipherMode.CBC };
        }

        private byte[] CreateCryptographyKey(string cryptoKey)
        {
            var base64String = Convert.ToBase64String(Encoding.Unicode.GetBytes(cryptoKey));

            return Encoding.UTF8.GetBytes(base64String);
        }
    }
}