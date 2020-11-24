﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.SignalR.Contracts;
using MassTransit.SignalR.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace SampleSignalR.Service
{
    static class Program
    {
        internal static async Task Main(string[] args)
        {
            IReadOnlyList<IHubProtocol> protocols = new IHubProtocol[]{new JsonHubProtocol(), };
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
            //must call before using this
            await busControl.StartAsync();
            do
            {
                Console.WriteLine("Enter message (or quit to exit)");
                Console.Write("> ");
                var value = Console.ReadLine();

                if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                    break;
                await busControl.Publish<All<ChatHub>>(new
                {
                    Messages = protocols.ToProtocolDictionary("broadcastMessage", new object[] { "backend-process", value })
                });
            } while (true);
            await busControl.StopAsync();
        }
    }

    public class ChatHub : Hub
    {
        // Actual implementation in the other project, but MT Needs the hub for the generic message type
    }
}