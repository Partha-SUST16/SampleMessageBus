using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyCompany.Messaging;

namespace MassTransit.Receiver
{
    public class RegisterCustomerFaultConsumer : IConsumer<Fault<IRegisterCustomer>>
    {
        public Task Consume(ConsumeContext<Fault<IRegisterCustomer>> context)
        {
            IRegisterCustomer originalFault = context.Message.Message;
            ExceptionInfo[] exceptions = context.Message.Exceptions;
            return Task.FromResult(originalFault);
        }
    }
}
