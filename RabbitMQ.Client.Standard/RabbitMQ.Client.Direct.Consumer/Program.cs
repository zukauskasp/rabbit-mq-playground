using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Direct.Consumer
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

            channel.ExchangeDeclare("ex.direct", "direct", true, false, null);
            channel.QueueDeclare("my.error", true, false, false, null);
            channel.QueueDeclare("my.warning", true, false, false, null);
            channel.QueueBind("my.error", "ex.direct", "error");
            channel.QueueBind("my.warning", "ex.direct", "warning");

            channel.BasicPublish("ex.direct", "error", null, Encoding.UTF8.GetBytes("Hi From .NET Standard to error queue"));
            channel.BasicPublish("ex.direct", "warning", null, Encoding.UTF8.GetBytes("Hi From .NET Standard to warning queue"));
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
