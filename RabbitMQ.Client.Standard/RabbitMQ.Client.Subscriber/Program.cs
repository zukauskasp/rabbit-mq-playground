using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Subscriber
{
    class Program
    {
        static IConnection conn;
        static IModel channel;
        static void Main(string[] args)
        {
            Console.WriteLine("Enter a queue name");
            string queueName = Console.ReadLine();
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();
            //  channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body);
                Console.Write("Started " + message);
                Console.Write(" Completed " + message);
                channel.BasicAck(e.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(queueName, false, consumer);
            Console.WriteLine($"Subscribed to  {queueName}, please press key to continue");
            Console.ReadKey();


            channel.Close();
            conn.Close();
        }
    }
}
