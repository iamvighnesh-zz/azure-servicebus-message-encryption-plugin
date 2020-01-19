using ServiceBus.MessageEncryption.Providers;

namespace ServiceBus.MessageEncryption
{
    public static class EncryptedMessagePayloadPluginExtensions
    {
        public static void RegisterMessageEncryptionPlugin(this Microsoft.Azure.ServiceBus.IClientEntity clientEntity, string cryptoKey)
        {
            ICryptographyProvider provider = new TripleDESCryptographyProvider(cryptoKey);

            clientEntity.RegisterPlugin(new EncryptedMessagePayloadPlugin(provider));
        }
    }
}
