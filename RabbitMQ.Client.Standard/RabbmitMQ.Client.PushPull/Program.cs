using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbmitMQ.Client.PushPull
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {

            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            //readMessagesWithPushModel();
            readMessagesWithPullModel();
            channel.Close();
            conn.Close();
        }

        public static void readMessagesWithPushModel()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine("Message " + message);
                
            };

            string consumerTag = channel.BasicConsume("my.queue", true, consumer);
            Console.WriteLine("Subscribed, please press key to continue");
            Console.ReadKey();
            channel.BasicCancel(consumerTag);
        }

        static void readMessagesWithPullModel()
        {
            while (true)
            {
                Console.WriteLine("Trying to get message from queue");
                BasicGetResult result = channel.BasicGet("my.queue", false);
                if (result != null) {
                    Console.WriteLine("Message "+ Encoding.UTF8.GetString(result.Body));
                    channel.BasicAck(result.DeliveryTag, false);
                }

                Thread.Sleep(2000);
            }
        }
    }
}
