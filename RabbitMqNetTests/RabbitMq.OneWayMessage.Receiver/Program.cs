using System;
using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMq.OneWayMessage.Receiver
{
    class Program
    {
        private static IModel channelForEventing;
        static void Main(string[] args)
        {
            ScatterGather();
            Console.ReadKey();
        }

        private static void ScatterGather()
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
            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);
            string consumerId = "A";
            Console.WriteLine(string.Concat("Consumer ", consumerId, " up and running, waiting for the publisher to start the bidding process."));
            eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                string message = (basicDeliveryEventArgs.Body.ToString());
                channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                Console.WriteLine("Message: {0} {1}", message, " Enter your response: ");
                string response = string.Concat("Consumer ID: ", consumerId, ", bid: ", Console.ReadLine());
                IBasicProperties replyBasicProperties = channel.CreateBasicProperties();
                replyBasicProperties.CorrelationId = basicDeliveryEventArgs.BasicProperties.CorrelationId;
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish("", basicDeliveryEventArgs.BasicProperties.ReplyTo, replyBasicProperties, responseBytes);
                channel.Close();
                connection.Close();
            };
            channel.BasicConsume("mycompany.queues.scattergather.a", false, eventingBasicConsumer);
        }
    
        private static void ReceiveRPCMessage()
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
            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);

            eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                string message = (basicDeliveryEventArgs.Body.ToString());
                channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
                Console.WriteLine("Message: {0} {1}", message, " Enter your response: ");
                string response = Console.ReadLine();
                IBasicProperties replyBasicProperties = channel.CreateBasicProperties();
                replyBasicProperties.CorrelationId = basicDeliveryEventArgs.BasicProperties.CorrelationId;
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                channel.BasicPublish("", basicDeliveryEventArgs.BasicProperties.ReplyTo, replyBasicProperties, responseBytes);
            };

            channel.BasicConsume("mycompany.queues.rpc", false, eventingBasicConsumer);
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
        private static void ReceiveMessagesWithEvents()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();

            connectionFactory.Port = 5672;
            connectionFactory.HostName = "localhost";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            connectionFactory.VirtualHost = "accounting";

            IConnection connection = connectionFactory.CreateConnection();
            channelForEventing = connection.CreateModel();
            channelForEventing.BasicQos(0, 1, false);
            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channelForEventing);
            eventingBasicConsumer.Received += EventingBasicConsumer_Received;
            channelForEventing.BasicConsume("my.first.queue", false, eventingBasicConsumer);

        }
        private static void EventingBasicConsumer_Received(object sender, BasicDeliverEventArgs e)
        {
            IBasicProperties basicProperties = e.BasicProperties;
            Console.WriteLine("Message received by the event based consumer. Check the debug window for details.");
            Debug.WriteLine(string.Concat("Message received from the exchange ", e.Exchange));
            Debug.WriteLine(string.Concat("Content type: ", basicProperties.ContentType));
            Debug.WriteLine(string.Concat("Consumer tag: ", e.ConsumerTag));
            Debug.WriteLine(string.Concat("Delivery tag: ", e.DeliveryTag));
            Debug.WriteLine(string.Concat("Message: ", e.Body.ToString()));
            channelForEventing.BasicAck(e.DeliveryTag, false);
        }
        private static void ReceiveFanoutMessages()
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
            EventingBasicConsumer eventingBasicConsumer = new EventingBasicConsumer(channel);

            eventingBasicConsumer.Received += (sender, basicDeliveryEventArgs) =>
            {
                IBasicProperties basicProperties = basicDeliveryEventArgs.BasicProperties;

                Debug.WriteLine(string.Concat("Message received from the exchange ", basicDeliveryEventArgs.Exchange));
                Debug.WriteLine(string.Concat("Content type: ", basicProperties.ContentType));
                Debug.WriteLine(string.Concat("Consumer tag: ", basicDeliveryEventArgs.ConsumerTag));
                Debug.WriteLine(string.Concat("Delivery tag: ", basicDeliveryEventArgs.DeliveryTag));
                string message = basicDeliveryEventArgs.Body.ToString();
                Debug.WriteLine(string.Concat("Message: ", message));
                Console.WriteLine(string.Concat("Message received by the accounting consumer: ", message));
                channel.BasicAck(basicDeliveryEventArgs.DeliveryTag, false);
            };

            channel.BasicConsume("mycompany.queues.accounting", false, eventingBasicConsumer);
        }
    }
}
