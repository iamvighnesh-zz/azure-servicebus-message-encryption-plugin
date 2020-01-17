using Azure.ServiceBus.MessageEncryption.Providers;
using System.Threading.Tasks;

namespace Azure.ServiceBus.MessageEncryption.Tests.TestDoubles
{
    public class FakeCryptographyProvider : ICryptographyProvider
    {
        public Task<byte[]> Decrypt(byte[] input)
        {
            var actualString = System.Text.Encoding.UTF8.GetString(input);

            var outputString = $"{actualString.Replace("-ENCRYPTED", string.Empty)}";

            var result = System.Text.Encoding.UTF8.GetBytes(outputString);

            return Task.FromResult(result);
        }

        public Task<byte[]> Encrypt(byte[] input)
        {
            var actualString = System.Text.Encoding.UTF8.GetString(input);

            var outputString = $"{actualString}-ENCRYPTED";

            var result = System.Text.Encoding.UTF8.GetBytes(outputString);

            return Task.FromResult(result);
        }
    }
}
