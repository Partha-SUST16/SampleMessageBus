using System;
using RabbitMQ.Client;

namespace RabbitMqNetTests
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri("amqp://guest:guest@localhost:5672/accounting");
            IConnection connection = connectionFactory.CreateConnection();
            Console.WriteLine(string.Concat("Connection open: ", connection.IsOpen));
        }
    }
}
