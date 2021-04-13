
using Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Request
{
    class Program
    {
        static IConnection conn;
        static IModel channel;

        public static void Main(string[] args)
        {
            ConcurrentDictionary<string, CalculationRequest> waitingRequest = new ConcurrentDictionary<string, CalculationRequest>();
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
                string requestId = Encoding.UTF8.GetString((byte[])e.BasicProperties.Headers[Constants.RequestIdHeaderKey]);

                CalculationRequest request;
                if (waitingRequest.TryGetValue(requestId, out request))
                {
                    string messageData = Encoding.UTF8.GetString(e.Body);
                    CalculationResponse response = JsonConvert.DeserializeObject<CalculationResponse>(messageData);
                    Console.WriteLine("Calculation request received " + response.ToString());
                }
                

                channel.BasicAck(e.DeliveryTag, false);
            };

            channel.BasicConsume("responses", false, consumer);

            Console.WriteLine("Press any key to start");
            Console.ReadKey();

            sendRequest(waitingRequest, channel, new CalculationRequest(2, 4, RequestType.Add));
            sendRequest(waitingRequest, channel, new CalculationRequest(2, 4, RequestType.Substract));

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            channel.Close();
            conn.Close();
        }

        private static void sendRequest(ConcurrentDictionary<string, CalculationRequest> waitingRequest
            , IModel channel, CalculationRequest request)
        {
            string requestId = Guid.NewGuid().ToString();
            string requestData = JsonConvert.SerializeObject(request);

            waitingRequest[requestId] = request;

            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>();
            basicProperties.Headers.Add(Constants.RequestIdHeaderKey, Encoding.UTF8.GetBytes(requestId));
            channel.BasicPublish("", "requests", basicProperties, Encoding.UTF8.GetBytes(requestData));
        }
    }
}
