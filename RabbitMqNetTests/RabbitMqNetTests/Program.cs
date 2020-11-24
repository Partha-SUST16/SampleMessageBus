using System;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMqNetTests
{
    class Program
    {
        static void Main(string[] args)
        {
            SetUpFanoutExchange();
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
