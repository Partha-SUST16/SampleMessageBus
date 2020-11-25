using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMqNetTests
{
    class Program
    {
        static void Main(string[] args)
        {
            RunScatterGatherQueue();
            Console.ReadKey();
        }
        private static void RunScatterGatherQueue()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();

            connectionFactory.Port = 5672;
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            connectionFactory.VirtualHost = "accounting";

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.QueueDeclare("mycompany.queues.scattergather.a", true, false, false, null);
            channel.QueueDeclare("mycompany.queues.scattergather.b", true, false, false, null);
            channel.QueueDeclare("mycompany.queues.scattergather.c", true, false, false, null);
            channel.ExchangeDeclare("mycompany.exchanges.scattergather", ExchangeType.Fanout, true, false, null);
            channel.QueueBind("mycompany.queues.scattergather.a", "mycompany.exchanges.scattergather", "");
            channel.QueueBind("mycompany.queues.scattergather.b", "mycompany.exchanges.scattergather", "");
            channel.QueueBind("mycompany.queues.scattergather.c", "mycompany.exchanges.scattergather", "");
            SendScatterGatherMessages(connection, channel, 3);
        }

        private static void SendScatterGatherMessages(IConnection connection, IModel channel, int minResponses)
        {
            List<string> responses = new List<string>();
            string rpcResponseQueue = channel.QueueDeclare().QueueName;
            string correlationId = Guid.NewGuid().ToString();

            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.ReplyTo = rpcResponseQueue;
            basicProperties.CorrelationId = correlationId;
            Console.WriteLine("Enter your message and press Enter.");

            string message = Console.ReadLine();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("mycompany.exchanges.scattergather", "", basicProperties, messageBytes);

            EventingBasicConsumer scatterGatherEventingBasicConsumer = new EventingBasicConsumer(channel);
            scatterGatherEventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                IBasicProperties props = basicDeliveryEventArgs.BasicProperties;
                channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                if (props != null
                    && props.CorrelationId == correlationId)
                {
                    string response = (basicDeliveryEventArgs.Body.ToString());
                    Console.WriteLine("Response: {0}", response);
                    responses.Add(response);
                    if (responses.Count >= minResponses)
                    {
                        Console.WriteLine(string.Concat("Responses received from consumers: ", string.Join(Environment.NewLine, responses)));
                        channel.Close();
                        connection.Close();
                    }
                }
            };
            channel.BasicConsume(rpcResponseQueue, false, scatterGatherEventingBasicConsumer);
        }
        private static void RunRpcQueue()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();

            connectionFactory.Port = 5672;
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            connectionFactory.VirtualHost = "accounting";

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.QueueDeclare("mycompany.queues.rpc", true, false, false, null);
            SendRpcMessagesBackAndForth(channel);

            channel.Close();
            connection.Close();
        }
        private static void SendRpcMessagesBackAndForth(IModel channel)
        {
            string rpcResponseQueue = channel.QueueDeclare().QueueName;

            string correlationId = Guid.NewGuid().ToString();
            string responseFromConsumer = null;

            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.ReplyTo = rpcResponseQueue;
            basicProperties.CorrelationId = correlationId;
            Console.WriteLine("Enter your message and press Enter.");
            string message = Console.ReadLine();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("", "mycompany.queues.rpc", basicProperties, messageBytes);

            EventingBasicConsumer rpcEventingBasicConsumer = new EventingBasicConsumer(channel);
            rpcEventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                IBasicProperties props = basicDeliveryEventArgs.BasicProperties;
                if (props != null
                    && props.CorrelationId == correlationId)
                {
                    string response = basicDeliveryEventArgs.Body.ToString();
                    responseFromConsumer = response;
                }
                channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                Console.WriteLine("Response: {0}", responseFromConsumer);
                Console.WriteLine("Enter your message and press Enter.");
                message = Console.ReadLine();
                messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("", "mycompany.queues.rpc", basicProperties, messageBytes);
            };
            channel.BasicConsume(rpcResponseQueue, false, rpcEventingBasicConsumer);
        }
        private static void SetUpFanoutExchange()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();

            connectionFactory.Port = 5672;
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            connectionFactory.VirtualHost = "accounting";

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();

            channel.ExchangeDeclare("mycompany.fanout.exchange", ExchangeType.Fanout, true, false, null);
            channel.QueueDeclare("mycompany.queues.accounting", true, false, false, null);
            channel.QueueDeclare("mycompany.queues.management", true, false, false, null);
            channel.QueueBind("mycompany.queues.accounting", "mycompany.fanout.exchange", "");
            channel.QueueBind("mycompany.queues.management", "mycompany.fanout.exchange", "");

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            PublicationAddress address = new PublicationAddress(ExchangeType.Fanout, "mycompany.fanout.exchange", "");
            channel.BasicPublish(address, properties, Encoding.UTF8.GetBytes("A new huge order has just come in worth $1M!!!!!"));

            channel.Close();
            connection.Close();
            Console.WriteLine(string.Concat("Channel is closed: ", channel.IsClosed));

            Console.WriteLine("Main done...");
        }
        private static void SetUpDirectExchange()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();

            connectionFactory.Port = 5672;
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            connectionFactory.VirtualHost = "accounting";

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            Console.WriteLine(string.Concat("Connection open: ", connection.IsOpen));

            channel.ExchangeDeclare("my.first.exchange", ExchangeType.Direct, true, false, null);
            channel.QueueDeclare("my.first.queue", true, false, false, null);
            channel.QueueBind("my.first.queue", "my.first.exchange", "");

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "text/plain";
            PublicationAddress address = new PublicationAddress(ExchangeType.Direct, "my.first.exchange", "");
            channel.BasicPublish(address, properties,
                Encoding.UTF8.GetBytes("This is a message from the RabbitMq .NET driver"));

            channel.Close();
            connection.Close();
            Console.WriteLine(string.Concat("Channel is closed: ", channel.IsClosed));

            Console.WriteLine("Main done...");
            Console.ReadKey();
        }
    }
}
