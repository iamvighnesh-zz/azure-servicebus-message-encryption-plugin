using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Azure.ServiceBus.MessageEncryption.Providers
{
    public class TripleDESCryptographyProvider : ICryptographyProvider
    {
        private readonly TripleDESCryptoServiceProvider cryptoServiceProvider;

        public TripleDESCryptographyProvider(string cryptoKey) : this(System.Text.Encoding.UTF8.GetBytes(cryptoKey)) { }

        public TripleDESCryptographyProvider(byte[] cryptoKeyBytes)
        {
            cryptoServiceProvider = new TripleDESCryptoServiceProvider
            {
                Key = cryptoKeyBytes,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
        }

        public async Task<byte[]> Decrypt(byte[] input)
        {
            var decryptor = cryptoServiceProvider.CreateDecryptor();

            using (var memoryStream = new MemoryStream(input))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        var destinationString = await streamReader.ReadToEndAsync();

                        return System.Text.Encoding.UTF8.GetBytes(destinationString);
                    }
                }
            }
        }

        public async Task<byte[]> Encrypt(byte[] input)
        {
            var encryptor = cryptoServiceProvider.CreateEncryptor();

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        await cryptoStream.WriteAsync(input);

                        return memoryStream.ToArray();
                    }
                }
            }
        }
    }
}