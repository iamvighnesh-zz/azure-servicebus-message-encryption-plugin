using Microsoft.Azure.ServiceBus.Core;
using System.Threading.Tasks;

namespace Azure.ServiceBus.Message.Encryption.Core
{
    public class EncryptedMessagePayloadPlugin : ServiceBusPlugin
    {
        public override string Name => nameof(EncryptedMessagePayloadPlugin);

        public override async Task<Microsoft.Azure.ServiceBus.Message> BeforeMessageSend(Microsoft.Azure.ServiceBus.Message message)
        {
            return await base.BeforeMessageSend(message);
        }
    }
}
