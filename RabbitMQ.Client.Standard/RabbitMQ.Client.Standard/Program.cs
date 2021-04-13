using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitMQ.Client.Standard
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

            channel.ExchangeDeclare("ex.fanout1", "fanout", true, false, null);
            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);
            channel.QueueBind("my.queue1", "ex.fanout1", "");
            channel.QueueBind("my.queue2", "ex.fanout1", "");

            channel.BasicPublish("ex.fanout1", "", null, Encoding.UTF8.GetBytes("Hi From .NET Standard"));
            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.fanout1");
            channel.Close();
            conn.Close();
        }
        }
}
