using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publisher
{
    class Program
    {
        static IConnection conn;
        static IModel channel;

        public static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.ExchangeDeclare("ex.fanout", "fanout", true, false,null);
            channel.QueueDeclare("my.queue", true, false, false, new Dictionary<string, object>() {
                {"x-max-priority", 2}
            });

            channel.QueueBind("my.queue", "ex.fanout", "");

            //var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += (sender, e) =>
            //{

 
            //        string messageData = Encoding.UTF8.GetString(e.Body);
            //        Console.WriteLine("Message received " + messageData);


            //    channel.BasicAck(e.DeliveryTag, false);
            //};


            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            sendRequest("Hi with priority 1", 1, channel);
            sendRequest("Hi with priority 1", 1, channel);
            sendRequest("Hi with priority 1", 1, channel);
            sendRequest("Hi with priority 2", 2, channel);
            sendRequest("Hi with priority 2", 2, channel);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            channel.Close();
            conn.Close();
        }

        private static void sendRequest(string messageToSend, byte priority, IModel channelt)
        {
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Priority = priority;

            channel.BasicPublish("ex.fanout", "", basicProperties, Encoding.UTF8.GetBytes(messageToSend));
        }
    }
}
