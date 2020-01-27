using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;
using ServiceBus.MessageEncryption.Tests.TestDoubles;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.MessageEncryption.Tests
{
    public class MessagePayloadEncryptionPluginShould
    {
        private MessagePayloadEncryptionPlugin plugin;

        [SetUp()]
        public void Setup()
        {
            plugin = new MessagePayloadEncryptionPlugin(new FakeCryptographyProvider());
        }

        [Test]
        public async Task Encrypt_Message_Payload_Before_Sending_The_Message()
        {
            //Arrange
            const string inputMessagePayload = "MESSAGE_PAYLOAD";
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes(inputMessagePayload));

            //Act
            var result = await plugin.BeforeMessageSend(serviceBusMessage);

            //Assert
            var actualMessagePayload = Encoding.UTF8.GetString(result.Body);
            actualMessagePayload.Should().Be($"{inputMessagePayload}-ENCRYPTED");
        }

        [Test]
        public async Task Decrypt_Message_Payload_After_Receiving_The_Message()
        {
            //Arrange
            const string inputMessagePayload = "MESSAGE_PAYLOAD-ENCRYPTED";
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes(inputMessagePayload));

            //Act
            var result = await plugin.AfterMessageReceive(serviceBusMessage);

            //Assert
            var actualMessagePayload = Encoding.UTF8.GetString(result.Body);
            actualMessagePayload.Should().Be("MESSAGE_PAYLOAD");
        }
    }
}