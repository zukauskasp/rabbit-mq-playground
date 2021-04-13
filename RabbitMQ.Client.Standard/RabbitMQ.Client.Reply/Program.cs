using Common;
using CommonLibs;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Reply
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
            consumer.Received += (sender, e) =>
            {
                Console.WriteLine("Request received");
                string message = Encoding.UTF8.GetString(e.Body);
                CalculationRequest request = JsonConvert.DeserializeObject<CalculationRequest>(message);

                CalculationService calcService = new CalculationService();
                var response = calcService.Calculate(request);
                var responseData = JsonConvert.SerializeObject(response);

                var basicProperties = channel.CreateBasicProperties();
                basicProperties.Headers = new Dictionary<string, object>();
                basicProperties.Headers.Add(Constants.RequestIdHeaderKey, e.BasicProperties.Headers[Constants.RequestIdHeaderKey]);
                Console.WriteLine("Request received " + message);
                channel.BasicAck(e.DeliveryTag, false);

                channel.BasicPublish("","responses", basicProperties, Encoding.UTF8.GetBytes(responseData));
            };

            string consumerTag = channel.BasicConsume("requests", false, consumer);
            Console.WriteLine("Please any key to exit");
            Console.ReadKey();

            channel.Close();
            conn.Close();
        }
    }
}
