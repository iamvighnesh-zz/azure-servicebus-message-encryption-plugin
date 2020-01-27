using Microsoft.Azure.ServiceBus;
using ServiceBus.MessageEncryption.Providers;

namespace ServiceBus.MessageEncryption
{
    public static class MessagePayloadEncryptionPluginExtensions
    {
        public static void WithRijndaelManagedEncryption(this IClientEntity clientEntity, string cryptoKey, string initVectorKey)
        {
            ICryptographyProvider provider = new RijndaelManagedCryptographyProvider(cryptoKey, initVectorKey);

            clientEntity.RegisterPlugin(new MessagePayloadEncryptionPlugin(provider));
        }
    }
}
