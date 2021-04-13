using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Client.AlternateExchange
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
            channel.ExchangeDeclare("ex.fanout", "fanout", true, false, null);
            channel.ExchangeDeclare("ex.direct", "direct", true, false, new Dictionary<string, object>() {
                {"alternate-exchange","ex.fanout" }
            });
            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);
            channel.QueueDeclare("my.unrouted", true, false, false, null);
            channel.QueueBind("my.queue1", "ex.direct", "video");
            channel.QueueBind("my.queue2", "ex.direct", "image");
            channel.QueueBind("my.unrouted", "ex.fanout", "");

            channel.BasicPublish("ex.direct", "video", null, Encoding.UTF8.GetBytes("Hi From .NET Standard to my.queue1"));
            channel.BasicPublish("ex.direct", "image", null, Encoding.UTF8.GetBytes("Hi From .NET Standard to my.queue2"));
            channel.BasicPublish("ex.direct", "sdf", null, Encoding.UTF8.GetBytes("Hi From .NET Standard to my.unrouted"));
            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.QueueDelete("my.unrouted");
            channel.ExchangeDelete("ex.fanout");
            channel.ExchangeDelete("ex.direct");
            channel.Close();
            conn.Close();
        }
    }
}
