using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MyCompany.Domains;
using MyCompany.Messaging;

namespace MassTransit.Receiver
{
    public class RegisterCustomerConsumer : IConsumer<IRegisterCustomer>
    {
        private readonly ICustomerRepository _customerRepository;

        public RegisterCustomerConsumer(ICustomerRepository customerRepository)
        {
            if (customerRepository == null) throw new ArgumentNullException("Customer repository");
            _customerRepository = customerRepository;
        }
        public Task Consume(ConsumeContext<IRegisterCustomer> context)
        {
            IRegisterCustomer newCustomer = context.Message;
            Debug.WriteLine("A new customer has signed up, it's time to register it. Details: ");
            Debug.WriteLine(newCustomer.Address);
            Debug.WriteLine(newCustomer.Name);
            Debug.WriteLine(newCustomer.Id);
            Debug.WriteLine(newCustomer.Preferred);

            _customerRepository.Save(new Customer(newCustomer.Id, newCustomer.Name, newCustomer.Address)
            {
                DefaultDiscount = newCustomer.DefaultDiscount,
                Preferred = newCustomer.Preferred,
                RegisteredUtc = newCustomer.RegisteredUtc,
                Type = newCustomer.Type
            });

            context.Publish<ICustomerRegistered>(new
            {
                Address = newCustomer.Address,
                Id = newCustomer.Id,
                RegisteredUtc = newCustomer.RegisteredUtc,
                Name = newCustomer.Name
            });


            return Task.FromResult(context.Message);
        }
    }
}
