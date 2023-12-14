using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Abstractions;
using Warehouse.Infrastructure.Abstraction;
using Warehouse.Infrastructure.Repositories;

namespace Warehouse.Infrastructure.Extensions;
public static class DependencyInjectionExtension
{

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        return services;

    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {

        var  a = configuration.GetConnectionString("Postgre");

        services.AddDbContext<PostgreDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Postgre"));
        });

        services.AddScoped<PostgreDbContext>();

        return services;

    }
}