using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MassTransit.Receiver
{
    public class BusObserver : IBusObserver
    {
        private readonly string _logger = "[BusObserver]";

        public async Task CreateFaulted(Exception exception)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus exception: ", exception.Message));
            await Task.FromResult(0);
        }

        public async Task PostCreate(IBus bus)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus has been created with address ", bus.Address));
            await Task.FromResult(0);
        }

        public async Task PostStart(IBus bus, Task busReady)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus has been started with address ", bus.Address));
            await busReady;
        }

        public async Task PostStop(IBus bus)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus has been stopped with address ", bus.Address));
            await Task.FromResult(0);
        }

        public async Task PreStart(IBus bus)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus is about to start with address ", bus.Address));
            await Task.FromResult(0);
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            throw new NotImplementedException();
        }

        public async Task PreStop(IBus bus)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus is about to stop with address ", bus.Address));
            await Task.FromResult(0);
        }

        public async Task StartFaulted(IBus bus, Exception exception)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus exception at start-up: ", exception.Message
                , " for bus ", bus.Address));
            await Task.FromResult(0);
        }

        public async Task StopFaulted(IBus bus, Exception exception)
        {
            Console.WriteLine(string.Concat(_logger, ": Bus exception at shut-down: ", exception.Message
                , " for bus ", bus.Address));
            await Task.FromResult(0);
        }
    }
}
