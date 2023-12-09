using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Abstractions;
using Warehouse.Infrastructure.Repositories;
using Forex.Infrastructure.RabbitMq;
using Forex.Infrastructure.RabbitMq.Extensions;
using MassTransit;
using Warehouse.Application.Saga;
using Warehouse.Application.EventHandlers;
using Warehouse.Application.Events;
namespace Warehouse.Application.Extensions
{
    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddKafka(this IServiceCollection services)
        {
            services.AddRabbitMq(c =>

                  c.AddBus<IBus>("Default")
                    .ConfigureBus(c =>
                    {
                        //c.AddConsumer<AdminCreatedEventHandler>();
                        //c.AddConsumer<CreateCurrencyCommandEventHandler>();
                        //c.AddConsumer<CreateOperationCommandEventHandler>();
                        //c.AddConsumer<OperationCreatedEventHandler>();
                        //c.AddConsumer<CreateTransactionCommandEventHandler>();

                        //c.SetMongoDbContextSagaRepositoryProvider();

                        c.AddSagaStateMachine<OrderProcessorStateMachine, OrderProcessingState>();

                    })
                    //.ConfigureRabbitMq((context, cfg) =>
                    //{
                    //    cfg.UseDeduplicationConsumeFilter<MongoDbProcessedMessageStorage>(context,
                    //        c => c.ExpiryTime = TimeSpan.FromMinutes(5));
                    //})
                    //.RegisterMessage<CreateAdminCommand>()
                    //.RegisterMessage<CurrencyCreatedEvent>()
                    //.RegisterMessage<CreateOperationCommand>()
                    //.RegisterMessage<OperationCreatedEvent>()
                    //.RegisterMessage<StartTransactionCreationCommand>()
                    //.RegisterMessage<CreateTransactionCommand>()
                    .RegisterMessage<OrderProcessedEvent>()

                    );
            return services;
        }

        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.Load("Warehouse.Application")));

            return services;

        }
    }
}
