﻿using System;
using MassTransit.RabbitMqTransport;

namespace MassTransit.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "This is the customer registration command receiver.";
            Console.WriteLine("CUSTOMER REGISTRATION COMMAND RECEIVER.");
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
                rabbit.ReceiveEndpoint("mycompany.domains.queues", conf =>
                {
                    conf.Consumer<RegisterCustomerConsumer>();
                });
            });
            rabbitBusControl.Start();
            Console.ReadKey();

            rabbitBusControl.Stop();
        }
    }
}
