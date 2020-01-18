using Azure.ServiceBus.MessageEncryption.Providers;

namespace Azure.ServiceBus.MessageEncryption
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
