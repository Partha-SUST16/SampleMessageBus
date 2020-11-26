using System;
using System.Threading.Tasks;
using MyCompany.Messaging;

namespace MassTransit.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("CUSTOMER REGISTRATION COMMAND PUBLISHER");
            Console.Title = "Publisher window";
            RunMassTransitPublisherWithRabbit();
        }

        private static void RunMassTransitPublisherWithRabbit()
        {
            string rabbitMqAddress = "rabbitmq://localhost:5672/accounting";
            string rabbitMqQueue = "mycompany.domains.queues";
            Uri rabbitMqRootUri = new Uri(rabbitMqAddress);

            IBusControl rabbitBusControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
            {
                rabbit.Host(rabbitMqRootUri, settings =>
                {
                    settings.Password("guest");
                    settings.Username("guest");
                });
            });
            Task<ISendEndpoint> sendEndPointTask =
                rabbitBusControl.GetSendEndpoint(new Uri(string.Concat(rabbitMqAddress, "/", rabbitMqQueue)));
            ISendEndpoint sendEndpoint = sendEndPointTask.Result;
            Task sendTask = sendEndpoint.Send<IRegisterCustomer>(new
            {
                Address = "New Street",
                Id = Guid.NewGuid(),
                Preferred = true,
                RegisteredUtc = DateTime.UtcNow,
                Name = "Nice people LTD",
                Type = 1,
                DefaultDiscount = 0
            });
            //Un comment in case of deal with failures 
           /* Task sendErrorTask = sendEndpoint.Send<IRegisterCustomer>(new
            {
                Address = "New Street",
                Id = Guid.NewGuid(),
                Preferred = true,
                RegisteredUtc = DateTime.UtcNow,
                Name = "Nice people LTD",
                Type = 1,
                DefaultDiscount = 0
            }, c => c.FaultAddress = new Uri("rabbitmq://localhost:5672/accounting/mycompany.queues.errors.newcustomers"));
           */
            Console.ReadKey();
        }
    }
}
