using System.Threading.Tasks;

namespace ServiceBus.MessageEncryption.Providers
{
    public interface ICryptographyProvider
    {
        Task<byte[]> Encrypt(byte[] input);

        Task<byte[]> Decrypt(byte[] input);
    }
}