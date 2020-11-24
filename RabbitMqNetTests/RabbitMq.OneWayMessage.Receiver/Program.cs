using System;
using RabbitMQ.Client;

namespace RabbitMq.OneWayMessage.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            ReceiveSingleOneWayMessage();
        }
        private static void ReceiveSingleOneWayMessage()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();

            connectionFactory.Port = 5672;
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            connectionFactory.VirtualHost = "accounting";

            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);
            DefaultBasicConsumer basicConsumer = new OneWayMessageReceiver(channel);
            channel.BasicConsume("my.first.queue", false, basicConsumer);

        }
    }
}
