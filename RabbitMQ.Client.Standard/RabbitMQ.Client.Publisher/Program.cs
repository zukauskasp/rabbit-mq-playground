using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Publisher
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

            while (true)
            {
                Console.WriteLine("Enter a message");
                string message = Console.ReadLine();

                channel.BasicPublish("ex.fanout", "", null, Encoding.UTF8.GetBytes(message));

                if (message == "break")
                {
                    break;
                }
            }

            channel.Close();
            conn.Close();
        }
    }
}
