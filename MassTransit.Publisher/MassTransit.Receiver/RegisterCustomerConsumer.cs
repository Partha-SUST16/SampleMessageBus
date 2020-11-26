﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MyCompany.Messaging;

namespace MassTransit.Receiver
{
    public class RegisterCustomerConsumer : IConsumer<IRegisterCustomer>
    {
        public Task Consume(ConsumeContext<IRegisterCustomer> context)
        {
            IRegisterCustomer newCustomer = context.Message;
            Debug.WriteLine("A new customer has signed up, it's time to register it. Details: ");
            Debug.WriteLine(newCustomer.Address);
            Debug.WriteLine(newCustomer.Name);
            Debug.WriteLine(newCustomer.Id);
            Debug.WriteLine(newCustomer.Preferred);
            return Task.FromResult(context.Message);
        }
    }
}
