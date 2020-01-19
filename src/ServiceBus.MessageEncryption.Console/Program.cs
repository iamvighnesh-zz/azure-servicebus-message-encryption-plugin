using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.MessageEncryption.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //All your configuration goes here.
            const string connectionString = "<INSERT_YOUR_CONNECTION_STRING_HERE>";
            const string topicPath = "<INSERT_YOUR_TOPIC_PATH_HERE>";
            const string subscriptionPath = "<INSERT_YOUR_SUBSCRIPTION_NAME_HERE>";
            const string encryptionKey = "<INSERT_YOUR_ENCRYPTION_KEY_HERE>";

            //Publish the message
            var messageBody = Encoding.UTF8.GetBytes("HERE_GOES_YOUR_MESSAGE_CONTENT");
            var publishMessage = new Message(messageBody);

            //Build a Message Sender with encryption plugin registration
            var sender = new MessageSender(connectionString, topicPath, RetryPolicy.Default);
            sender.RegisterMessageEncryptionPlugin(encryptionKey);

            await sender.SendAsync(publishMessage);
            System.Console.WriteLine($"Published Message : {Encoding.UTF8.GetString(publishMessage.Body)}");

            Thread.Sleep(100);

            //Build a Message Receiver with encryption plugin registration
            var receiver = new MessageReceiver(connectionString, subscriptionPath, ReceiveMode.PeekLock, RetryPolicy.Default);
            receiver.RegisterMessageEncryptionPlugin(encryptionKey);
            var receivedMessage = await receiver.PeekAsync();
            System.Console.WriteLine($"Received Message : {Encoding.UTF8.GetString(receivedMessage.Body)}");
        }
    }
}
