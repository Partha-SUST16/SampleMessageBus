using System;
using MassTransit.RabbitMqTransport;

namespace MassTransit.Receiver.Sales
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Sales consumer";
            Console.WriteLine("SALES");
            RunMassTransitReceiverWithRabbit();
        }
        private static void RunMassTransitReceiverWithRabbit()
        {
            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(new Uri("rabbitmq://localhost:5672/accounting"), settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });

                rabbit.ReceiveEndpoint("mycompany.domains.queues.events.sales", conf =>
                {
                    conf.Consumer<CustomerRegisteredConsumerSls>();
                });
            });

            rabbitBusControl.Start();
            Console.ReadKey();

            rabbitBusControl.Stop();
        }
    }
}
