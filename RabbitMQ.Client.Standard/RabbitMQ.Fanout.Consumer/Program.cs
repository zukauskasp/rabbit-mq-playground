using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Fanout.Consumer
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
            // Second parameter means ack set to false. Once message is consumed it needs to be acknowledged
            var consumerTag = channel.BasicConsume("my.queue1", false, consumer); 
            Console.WriteLine("Waiting for messages. Press any key to exit");
            Console.ReadKey();
        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine("Message received "+ message + " Deliver tag: " + e.DeliveryTag);
            // Acknowledging message
            channel.BasicAck(e.DeliveryTag, false);

        }
    }
}
