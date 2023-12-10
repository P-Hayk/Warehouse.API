using System;
using Forex.Infrastructure.RabbitMq.Abstractions;
using MassTransit;
using MassTransit.Clients;
using MassTransit.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Application.Extensions
{
    internal class RequestClientAccessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBusResolver _busResolver;

        public RequestClientAccessor(
            IServiceProvider serviceProvider,
            IBusResolver busResolver)
        {
            _serviceProvider = serviceProvider;
            _busResolver = busResolver;
        }

        public IRequestClient<T> Get<T>(TimeSpan? timeout)
            where T : class
        {
            var requestTimeout = timeout != null
                ? RequestTimeout.After(
                timeout.Value.Days,
                timeout.Value.Hours,
                timeout.Value.Minutes,
                timeout.Value.Seconds,
                timeout.Value.Milliseconds)
                : default;

            var bus = _busResolver.Get<T>();

            var clientFactory = GetClientFactory(bus, requestTimeout);
            var consumeContext = _serviceProvider.GetRequiredService<ScopedConsumeContextProvider>().GetContext();

            if (consumeContext != null)
                return clientFactory.CreateRequestClient<T>(consumeContext, requestTimeout);

            var context = new ScopedClientFactoryContext<IServiceProvider>(clientFactory, _serviceProvider);
            return new ClientFactory(context)
                .CreateRequestClient<T>(requestTimeout);
        }

        protected IClientFactory GetClientFactory<TBus>(TBus bus, RequestTimeout requestTimeout)
            where TBus : class, IBus
        {
            return bus.CreateClientFactory(requestTimeout);
        }
    }
}
