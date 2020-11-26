using System;
using MassTransit.RabbitMqTransport;
using MyCompany.Domains;
using StructureMap;

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
            var container = new Container(conf =>
            {
                conf.For<ICustomerRepository>().Use<CustomerRepository>();
            });
            string whatDoIHave = container.WhatDoIHave();

            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(new Uri("rabbitmq://localhost:5672/accounting"), settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });

                rabbit.ReceiveEndpoint( "mycompany.domains.queues", conf =>
                {
                    conf.Consumer<RegisterCustomerConsumer>(container);
                    conf.Consumer<RegisterCustomerConsumer>(container);
                    conf.Consumer<RegisterDomainConsumer>();
                });
                rabbit.ReceiveEndpoint( "mycompany.queues.errors.newcustomers", conf =>
                {
                    conf.Consumer<RegisterCustomerFaultConsumer>();
                });
            });
           // rabbitBusControl.ConnectSendObserver(new SendObjectObserver());
            rabbitBusControl.Start();
            //rabbitBusControl.ConnectReceiveObserver(new MessageReceiveObserver());
            rabbitBusControl.ConnectConsumeObserver(new MessageConsumeObserver());
            Console.ReadKey();

            rabbitBusControl.Stop();
        }
    }
}
