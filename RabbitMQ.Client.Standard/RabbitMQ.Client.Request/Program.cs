using Common;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Request
{
    class Program
    {
        static IConnection conn;
        static IModel channel;

        public static void Main(string[] args)
        {
            ConcurrentDictionary<string, CalculationRequest> waitingRequest = new ConcurrentDictionary<string, CalculationRequest>();
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine("Message received " + message);
                
                channel.BasicAck(e.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume("responses", false, consumer);

            while (true)
            {
                Console.WriteLine("Enter your request");
                string request = Console.ReadLine();
                channel.BasicPublish("", "requests", null, Encoding.UTF8.GetBytes(request));

                if (request == "exist")
                {
                    break;
                }
            }

            channel.Close();
            conn.Close();
        }
    }
}
