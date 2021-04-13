using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.DefaultExchange.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            IConnection conn;
            IModel channel;
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            factory.VirtualHost = "/";
            factory.Port = 5672;
            factory.UserName = "guest";
            factory.Password = "guest";

            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.QueueDeclare("my.queue1", true, false, false, null);

            channel.BasicPublish("", "my.queue1", null, Encoding.UTF8.GetBytes("Hi From .NET Standard to queue 1"));

            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");

            channel.Close();
            conn.Close();
        }
    }
}
