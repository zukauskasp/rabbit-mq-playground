using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worker
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

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {


                string messagedata = Encoding.UTF8.GetString(e.Body);
                Console.WriteLine("message received " + messagedata);
                Thread.Sleep(1000);
                Console.WriteLine("FINISHED");

                channel.BasicAck(e.DeliveryTag, false);
            };

            string consumerTag = channel.BasicConsume(
                "my.queue", // qqueue name
                false, // auto ackl
                consumer);

            Console.WriteLine("Subscribed to queue");
            Console.ReadKey();

            channel.Close();
            conn.Close();
        }
    }
    
}
