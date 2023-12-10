using System;
using Forex.Infrastructure.RabbitMq.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Application.Extensions
{
    internal class BusResolver : IBusResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageBusTypeSource _messageBusTypeSource;

        public BusResolver(
            IServiceProvider serviceProvider,
            IMessageBusTypeSource messageBusTypeSource)
        {
            _serviceProvider = serviceProvider;
            _messageBusTypeSource = messageBusTypeSource;
        }

        public IBus Get<TMessage>()
            where TMessage : class
        {
            return Get<TMessage>(out _);
        }

        public IBus Get<TMessage>(out string busKey)
            where TMessage : class
        {
            var busInfo = _messageBusTypeSource.GetMessageBusInfo<TMessage>();
            busKey = busInfo.BusKey;
            return (IBus)_serviceProvider.GetRequiredService(busInfo.BusType);
        }

        public IBus Get(string name)
        {
            return Get(name, out _);
        }

        public IBus Get(string name, out string busKey)
        {
            var busInfo = _messageBusTypeSource.GetMessageBusInfo(name);
            busKey = busInfo.BusKey;
            return (IBus)_serviceProvider.GetRequiredService(busInfo.BusType);
        }
    }
}
