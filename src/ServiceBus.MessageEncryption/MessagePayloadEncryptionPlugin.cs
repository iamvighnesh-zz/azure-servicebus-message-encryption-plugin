using Microsoft.Azure.ServiceBus.Core;
using ServiceBus.MessageEncryption.Providers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.MessageEncryption
{
    public class MessagePayloadEncryptionPlugin : ServiceBusPlugin
    {
        private readonly ICryptographyProvider cryptographyProvider;

        public MessagePayloadEncryptionPlugin(ICryptographyProvider cryptographyProvider)
        {
            this.cryptographyProvider = cryptographyProvider;
        }

        public override string Name => typeof(MessagePayloadEncryptionPlugin).FullName;

        public override async Task<Microsoft.Azure.ServiceBus.Message> BeforeMessageSend(Microsoft.Azure.ServiceBus.Message message)
        {
            message.Body = await cryptographyProvider.Encrypt((byte[])message.Body.Clone());

            var encryptedBody = Encoding.UTF8.GetString(message.Body);

            return await base.BeforeMessageSend(message);
        }

        public override async Task<Microsoft.Azure.ServiceBus.Message> AfterMessageReceive(Microsoft.Azure.ServiceBus.Message message)
        {
            var encryptedBody = Encoding.UTF8.GetString(message.Body);

            message.Body = await cryptographyProvider.Decrypt(message.Body);

            return await base.AfterMessageReceive(message);
        }
    }
}