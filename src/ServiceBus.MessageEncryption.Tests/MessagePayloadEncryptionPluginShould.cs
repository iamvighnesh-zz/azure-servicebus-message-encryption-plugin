using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using NUnit.Framework;
using ServiceBus.MessageEncryption.Providers;
using ServiceBus.MessageEncryption.Tests.TestDoubles;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.MessageEncryption.Tests
{
    public class MessagePayloadEncryptionPluginShould
    {
        private ICryptographyProvider fakeCryptographyProvider;
        private const string integrityCheckKeyProperty = "int-check-key";
        private const string integrityCheckStatusProperty = "int-check-status";

        [SetUp()]
        public void Setup()
        {
            fakeCryptographyProvider = new FakeCryptographyProvider();
        }

        [Test]
        public async Task Encrypt_Message_Payload_Before_Sending_The_Message()
        {
            //Arrange
            const string inputMessagePayload = "MESSAGE_PAYLOAD";
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes(inputMessagePayload));

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider);

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

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider);

            //Act
            var result = await plugin.AfterMessageReceive(serviceBusMessage);

            //Assert
            var actualMessagePayload = Encoding.UTF8.GetString(result.Body);
            actualMessagePayload.Should().Be("MESSAGE_PAYLOAD");
        }

        [Test]
        public async Task Add_IntegrityCheck_Key_To_User_Properties_Before_Sending_The_Message()
        {
            //Arrange
            var messagePayloadBytes = Encoding.UTF8.GetBytes("MESSAGE_PAYLOAD");
            Message serviceBusMessage = new Message(messagePayloadBytes);

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider);

            //Act
            var result = await plugin.BeforeMessageSend(serviceBusMessage);

            //Assert
            result.UserProperties.ContainsKey(integrityCheckKeyProperty).Should().BeTrue();
            result.UserProperties[integrityCheckKeyProperty].ToString().Should().Be(GetMd5Hash(messagePayloadBytes));
        }

        [Test]
        public async Task Add_IntegrityCheck_Status_To_User_Properties_After_Receiving_The_Message()
        {
            //Arrange
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes("MESSAGE_PAYLOAD"));

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider);

            //Act
            var result = await plugin.BeforeMessageSend(serviceBusMessage);
            result = await plugin.AfterMessageReceive(result);

            //Assert
            result.UserProperties.ContainsKey(integrityCheckStatusProperty).Should().BeTrue();
            result.UserProperties[integrityCheckStatusProperty].ToString().Should().Be("True");
        }

        [Test]
        public async Task Allow_Overriding_User_Property_Name_For_Integrity_Check_Key()
        {
            //Arrange
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes("MESSAGE_PAYLOAD"));

            const string keyName = "intcheckkey";

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider, intCheckKeyPropertyName: keyName);

            //Act
            var result = await plugin.BeforeMessageSend(serviceBusMessage);

            //Assert
            result.UserProperties.ContainsKey(integrityCheckKeyProperty).Should().BeFalse();
            result.UserProperties.ContainsKey(keyName).Should().BeTrue();
        }

        [Test]
        public async Task Allow_Overriding_User_Property_Name_For_Integrity_Check_Status()
        {
            //Arrange
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes("MESSAGE_PAYLOAD"));

            const string keyName = "intcheckstatus";

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider, intCheckStatusPropertyName: keyName);

            //Act
            var result = await plugin.BeforeMessageSend(serviceBusMessage);
            result = await plugin.AfterMessageReceive(result);

            //Assert
            result.UserProperties.ContainsKey(integrityCheckStatusProperty).Should().BeFalse();
            result.UserProperties.ContainsKey(keyName).Should().BeTrue();
        }

        [Test]
        public async Task Throw_NullReferenceException_When_IntegrationCheckKey_Is_Missing_And_ThrowExeption_Flag_Is_True()
        {
            //Arrange
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes("MESSAGE_PAYLOAD"));

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider, throwExceptionWhenMissingProperties: true);

            //Act

            Func<Task<Message>> func = async () => await plugin.AfterMessageReceive(serviceBusMessage);

            await func.Should().ThrowExactlyAsync<NullReferenceException>();
        }

        [Test]
        public async Task Not_Add_IntegrityCheck_Status_To_User_Properties_When_IntegrationCheckKey_Is_Missing_And_ThrowExeption_Flag_Is_False()
        {
            //Arrange
            Message serviceBusMessage = new Message(Encoding.UTF8.GetBytes("MESSAGE_PAYLOAD"));

            var plugin = new MessagePayloadEncryptionPlugin(fakeCryptographyProvider);

            //Act
            var result = await plugin.AfterMessageReceive(serviceBusMessage);

            result.UserProperties.ContainsKey(integrityCheckStatusProperty).Should().BeFalse();
        }

        private string GetMd5Hash(byte[] bytes)
        {
            return Encoding.UTF8.GetString(MD5.Create().ComputeHash(bytes));
        }
    }
}