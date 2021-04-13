using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Headers.Consumer
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

            channel.ExchangeDeclare("ex.header", "headers", true, false, null);
            channel.QueueDeclare("my.queue.header1", true, false, false, null);
            channel.QueueDeclare("my.queue.header2", true, false, false, null);
            channel.QueueBind("my.queue.header1", "ex.header", "", new Dictionary<string, object>() {
                {"x-match","any"},
                {"job","format" },
                {"format","jpeg" }
            });
            channel.QueueBind("my.queue.header2", "ex.header", "", new Dictionary<string, object>() {
                {"x-match","all"},
                {"job","format" },
                {"format","bpm" }
            });

            IBasicProperties props1 = channel.CreateBasicProperties();
            props1.Headers = new Dictionary<string, object>();
            props1.Headers.Add("job", "format");
            props1.Headers.Add("format", "jpeg");

            IBasicProperties props2 = channel.CreateBasicProperties();
            props2.Headers = new Dictionary<string, object>();
            props2.Headers.Add("job", "format");
            props2.Headers.Add("format", "bpm");

            channel.BasicPublish("ex.header", "", props1, Encoding.UTF8.GetBytes("Hi From .NET Standard with props 1"));
            channel.BasicPublish("ex.header", "", props2, Encoding.UTF8.GetBytes("Hi From .NET Standard with props 2"));
            Console.WriteLine("Press a key to exit");
            Console.ReadKey();

            channel.QueueDelete("my.queue.header1");
            channel.QueueDelete("my.queue.header2");
            channel.ExchangeDelete("ex.header");
            channel.Close();
            conn.Close();
        }
    }
}
