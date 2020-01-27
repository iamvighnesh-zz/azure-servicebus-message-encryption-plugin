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
            const string cryptoKey = "<INSERT_YOUR_ENCRYPTION_KEY_HERE>";
            const string initVectorKey = "<INSERT_YOUR_INIT_VECTOR_KEY_HERE>";

            //Publish the message
            var messageJsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(new { FirstName = "Vighneshwar", LastName = "Madas" });
            var messageBody = Encoding.UTF8.GetBytes(messageJsonBody);
            var publishMessage = new Message(messageBody);

            //Build a Message Sender with encryption plugin registration
            var sender = new MessageSender(connectionString, topicPath, RetryPolicy.Default);
            sender.WithRijndaelManagedEncryption(cryptoKey, initVectorKey);

            await sender.SendAsync(publishMessage);
            System.Console.WriteLine($"Published Message : {Encoding.UTF8.GetString(publishMessage.Body)}");

            Thread.Sleep(100);

            //Build a Message Receiver with encryption plugin registration
            var receiver = new MessageReceiver(connectionString, subscriptionPath, ReceiveMode.PeekLock, RetryPolicy.Default);
            receiver.WithRijndaelManagedEncryption(cryptoKey, initVectorKey);

            var receivedMessage = await receiver.ReceiveAsync();
            System.Console.WriteLine($"Received Message : {Encoding.UTF8.GetString(receivedMessage.Body)}");
        }
    }
}
