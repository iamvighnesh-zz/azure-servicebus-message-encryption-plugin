using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using ServiceBus.MessageEncryption.Providers;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.MessageEncryption
{
    public class MessagePayloadEncryptionPlugin : ServiceBusPlugin
    {
        private readonly ICryptographyProvider cryptographyProvider;

        private readonly string intCheckKeyPropertyName;
        private readonly string intCheckStatusPropertyName;
        private readonly bool throwExceptionWhenMissingProperties;

        public MessagePayloadEncryptionPlugin(ICryptographyProvider cryptographyProvider,
            string intCheckKeyPropertyName = "int-check-key",
            string intCheckStatusPropertyName = "int-check-status",
            bool throwExceptionWhenMissingProperties = false)
        {
            this.cryptographyProvider = cryptographyProvider;
            this.intCheckKeyPropertyName = intCheckKeyPropertyName;
            this.intCheckStatusPropertyName = intCheckStatusPropertyName;
            this.throwExceptionWhenMissingProperties = throwExceptionWhenMissingProperties;
        }

        public override string Name => typeof(MessagePayloadEncryptionPlugin).FullName;

        public override async Task<Message> BeforeMessageSend(Message message)
        {
            AddIntegrityCheckKeyHeader(message);

            await EncryptMessageBody(message).ConfigureAwait(false);

            return await base.BeforeMessageSend(message).ConfigureAwait(false);
        }

        public override async Task<Message> AfterMessageReceive(Message message)
        {
            await DecryptMessageBody(message).ConfigureAwait(false);

            AddIntegrityCheckStatusHeader(message);

            return await base.AfterMessageReceive(message).ConfigureAwait(false);
        }

        private async Task EncryptMessageBody(Message message)
        {
            message.Body = await cryptographyProvider.Encrypt(message.Body).ConfigureAwait(false);
        }

        private async Task DecryptMessageBody(Message message)
        {
            message.Body = await cryptographyProvider.Decrypt(message.Body).ConfigureAwait(false);
        }

        private void AddIntegrityCheckKeyHeader(Message message)
        {
            var bodyHash = GetMd5Hash(message.Body);

            message.UserProperties.Add(intCheckKeyPropertyName, bodyHash);
        }

        private void AddIntegrityCheckStatusHeader(Message message)
        {
            if (!message.UserProperties.ContainsKey(intCheckKeyPropertyName))
            {
                if (throwExceptionWhenMissingProperties)
                {
                    throw new NullReferenceException($"A mandatory user property '{intCheckKeyPropertyName}' is missing from the message.");
                }

                return;
            }

            var integrityCheck = string.Equals(GetMd5Hash(message.Body), message.UserProperties[intCheckKeyPropertyName].ToString(), StringComparison.InvariantCultureIgnoreCase);

            message.UserProperties[intCheckStatusPropertyName] = integrityCheck;
        }

        private string GetMd5Hash(byte[] bytes)
        {
            var plainText = Encoding.UTF8.GetString(bytes);

            var hashBytes = Encoding.UTF8.GetBytes(plainText.Trim('\0'));

            return Encoding.UTF8.GetString(MD5.Create().ComputeHash(hashBytes));
        }
    }
}