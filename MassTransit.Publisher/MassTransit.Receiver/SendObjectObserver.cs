using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Receiver
{
    public class SendObjectObserver : ISendObserver
    {
        private readonly string _logger = "[SendObjectObserver]";
        public async Task PostSend<T>(SendContext<T> context) where T : class
        {
            Console.WriteLine(string.Concat(_logger, ": A message is post the sending stage for type ", context.Message.GetType()));
            await Task.FromResult(0);
        }

        public async Task PreSend<T>(SendContext<T> context) where T : class
        {
            Console.WriteLine(string.Concat(_logger, ": A message is before the sending stage for type ", context.Message.GetType()));
            await Task.FromResult(0);
        }

        public async Task SendFault<T>(SendContext<T> context, Exception exception) where T : class
        {
            Console.WriteLine(string.Concat(_logger, ": A message fault is sent for the type ", context.Message.GetType()
                , ", exception: ", exception.Message));
            await Task.FromResult(0);
        }
    }
}
