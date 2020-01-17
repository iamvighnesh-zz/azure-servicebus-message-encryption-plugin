using System.Threading.Tasks;

namespace Azure.ServiceBus.Message.Encryption.Core.Providers
{
    public interface ICryptographyProvider
    {
        Task<byte[]> Encrypt(byte[] input);

        Task<byte[]> Decrypt(byte[] input);
    }
}