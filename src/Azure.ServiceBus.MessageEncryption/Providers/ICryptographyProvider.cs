using System.Threading.Tasks;

namespace Azure.ServiceBus.MessageEncryption.Providers
{
    public interface ICryptographyProvider
    {
        Task<byte[]> Encrypt(byte[] input);

        Task<byte[]> Decrypt(byte[] input);
    }
}