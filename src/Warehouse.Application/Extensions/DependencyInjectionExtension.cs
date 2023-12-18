using Forex.Infrastructure.Correlation;
using Forex.Infrastructure.RabbitMq;
using Forex.Infrastructure.RabbitMq.Abstractions;
using Forex.Infrastructure.RabbitMq.Configurators;
using Forex.Infrastructure.RabbitMq.Internal.Filters;
using Forex.Infrastructure.RabbitMq.Internal.Observers;
using Forex.Infrastructure.RabbitMq.Options;
using Forex.Infrastructure.RabbitMq.Storages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Security.Authentication;
using Warehouse.Application.Consumers;
using Warehouse.Application.Events;
using Warehouse.Application.Messages;
using Warehouse.Application.Saga;
using Warehouse.Infrastructure;
using Warehouse.Infrastructure.Saga;
namespace Warehouse.Application.Extensions
{
    public static class DependencyInjectionExtension
    {

        public static IServiceCollection AddRabbitMq_SourceCode(
            this IServiceCollection services,
            Action<IRabbitMqMultiBusesConfigurator> rabbitMqMultiBusesConfiguratorAction)
        {
            var rabbitMqMultiBusesConfigurator = new RabbitMqMultiBusesConfigurator();
            rabbitMqMultiBusesConfiguratorAction?.Invoke(rabbitMqMultiBusesConfigurator);

            if (!rabbitMqMultiBusesConfigurator.MultiBusConfigurators.Any())
                throw new InvalidOperationException("No buses registered.");

            services.TryAddSingleton<IEndpointNameFormatter, ApplicationEndpointNameFormatter>();

            services.AddSingleton<IMessageBusTypeSource>(rabbitMqMultiBusesConfigurator);
            services.AddSingleton<IBusResolver, BusResolver>();

            services.AddOptions<RabbitMqOptions>()
                .Validate(c => !string.IsNullOrEmpty(c.ConnectionString), "Connection string is empty.");
            services.ConfigureOptions<RabbitMqOptionsConfigure>();

            services.AddOptions<RabbitMqConfigurationOptions>()
                .Validate(c => !string.IsNullOrWhiteSpace(c.EndpointPrefix), "EndpointName prefix is empty.");
            services.ConfigureOptions<RabbitMqConfigurationOptionsConfigure>();

            services.AddOptions<RabbitMqReceiveEndpointOptions>();
            services.ConfigureOptions<RabbitMqReceiveEndpointOptionsConfigure>();

            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<RequestClientAccessor>();
            services.AddScoped<IRequestSender, RequestSender>();

            services.AddSingleton<Func<ICorrelationContextFactory>>(c => c.GetService<ICorrelationContextFactory>);

            foreach (var multiBusConfigurator in rabbitMqMultiBusesConfigurator.MultiBusConfigurators)
            {
                AddMassTransitBus(services, multiBusConfigurator);
            }

            return services;
        }

        private static IServiceCollection AddMassTransitBus<TBus>(
        IServiceCollection services,
        RabbitMqMultiBusConfigurator<TBus> busConfigurator)
        where TBus : class, IBus
        {

            services.AddMassTransit<TBus>(c =>
            {
                busConfigurator.Invoke(c);

                c.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitMqOptions = context.GetRequiredService<IOptionsMonitor<RabbitMqOptions>>().Get(busConfigurator.BusKey);
                    var hostingEnv = context.GetRequiredService<IHostEnvironment>();

                    var uri = new Uri(rabbitMqOptions.ConnectionString);
                    cfg.Host(uri, rabbitHostConfiguration =>
                    {
                        rabbitHostConfiguration.PublisherConfirmation = true;

                        if (string.Equals(uri.Scheme, "amqps", StringComparison.InvariantCultureIgnoreCase))
                        {
                            rabbitHostConfiguration.UseSsl(policy =>
                            {
                                policy.Protocol = SslProtocols.Tls12;
                            });
                        }
                    });

                    cfg.AutoStart = true;

                    cfg.UseNewtonsoftJsonSerializer();

                    //todo: replace with AppMetrics implementation. Current nuget packages use prometheus-net.
                    cfg.UsePrometheusMetrics(serviceName: $"{hostingEnv.ApplicationName}_{hostingEnv.EnvironmentName}");

                    // cfg.UseSendFilter(typeof(CorrelationContextFilter<>), context);
                    //cfg.UsePublishFilter(typeof(CorrelationContextFilter<>), context);
                    //cfg.UseConsumeFilter(typeof(CorrelationContextFilter<>), context);

                    cfg.UseSendFilter(typeof(LoggerSendFilter<>), context);
                    //cfg.UsePublishFilter(typeof(LoggerPublishFilter<>), context);
                    //cfg.UseConsumeFilter(typeof(LoggerConsumeFilter<>), context);

                    cfg.ConnectEndpointConfigurationObserver(new ErrorObserver());
                    cfg.ConnectEndpointConfigurationObserver(ActivatorUtilities.CreateInstance<EndpointOptionsConfigurationObserver>(context, context));

                    cfg.ConfigureEndpoints(context);
                    busConfigurator.Invoke(context, cfg);

                    cfg.SendTopology.ConfigureErrorSettings = settings =>
                    {
                        settings.Lazy = true;
                        settings.SetQueueArgument("x-queue-type", "classic");
                    };

                    cfg.SendTopology.ConfigureDeadLetterSettings = settings =>
                    {
                        settings.Lazy = true;
                        settings.SetQueueArgument("x-queue-type", "classic");
                    };
                });


            });

            return services;
        }

        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddSingleton<InMemoryProcessedMessageStorage>();

            services.AddRabbitMq_SourceCode(c =>

                  c.AddBus<IBus>("Default")
                    .ConfigureBus(c =>
                    {
                        c.AddConsumer<OrderProcessConsumer>();
                        c.AddConsumer<OrderReserveConsumer>();
                        //c.AddConsumer<CreateOperationCommandEventHandler>();
                        //c.AddConsumer<OperationCreatedEventHandler>();
                        //c.AddConsumer<CreateTransactionCommandEventHandler>();

                        //c.SetMongoDbContextSagaRepositoryProvider();

                        c.SetInMemorySagaRepositoryProvider();

                        c.AddSagaStateMachine<OrderProcessorStateMachine, OrderProcessingState>().EntityFrameworkRepository(r =>
                        {
                            r.ConcurrencyMode = ConcurrencyMode.Optimistic; // or use Pessimistic, which does not require RowVersion

                            r.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                            {
                                builder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=admin;Database=Warehouse;", m =>
                                {
                                    m.MigrationsAssembly("Warehouse.Infrastructure");
                                    m.MigrationsHistoryTable($"__EFMigrationsHistory");
                                });
                            });

                            //This line is added to enable PostgreSQL features
                            r.UsePostgres();
                        });

                    })
                    //.ConfigureRabbitMq((context, cfg) =>
                    //{
                    //    cfg.UseDeduplicationConsumeFilter<MongoDbProcessedMessageStorage>(context,
                    //        c => c.ExpiryTime = TimeSpan.FromMinutes(5));
                    //})
                   
                    .RegisterMessage<OrderProcessMessage>()
                    //.RegisterMessage<OrderReserveMessage>()
                    .RegisterMessage<OrderSubmittedEvent>()
                    .RegisterMessage<OrderApprovedEvent>()
                     .RegisterMessage<OrderReservedEvent>()
                    .RegisterMessage<OrderRejectedEvent>()

                    );


            services.AddScoped<OrderSubmittedEvent>();
            services.AddScoped<OrderApprovedEvent>();
            services.AddScoped<OrderReservedEvent>();
            return services;
        }

        public static IServiceCollection AddMediatR(this IServiceCollection services, string assemblyString)
        {
            services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.Load(assemblyString)));

            return services;

        }

       
    }
}
