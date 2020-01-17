﻿using Azure.ServiceBus.Message.Encryption.Core.Providers;
using Microsoft.Azure.ServiceBus.Core;
using System.Threading.Tasks;

namespace Azure.ServiceBus.Message.Encryption.Core
{
    public class EncryptedMessagePayloadPlugin : ServiceBusPlugin
    {
        private readonly ICryptographyProvider cryptographyProvider;

        internal EncryptedMessagePayloadPlugin(ICryptographyProvider cryptographyProvider)
        {
            this.cryptographyProvider = cryptographyProvider;
        }

        public override string Name => typeof(EncryptedMessagePayloadPlugin).FullName;

        public override async Task<Microsoft.Azure.ServiceBus.Message> BeforeMessageSend(Microsoft.Azure.ServiceBus.Message message)
        {
            message.Body = await cryptographyProvider.Encrypt(message.Body);

            return await base.BeforeMessageSend(message);
        }

        public override async Task<Microsoft.Azure.ServiceBus.Message> AfterMessageReceive(Microsoft.Azure.ServiceBus.Message message)
        {
            message.Body = await cryptographyProvider.Decrypt(message.Body);

            return await base.AfterMessageReceive(message);
        }
    }
}