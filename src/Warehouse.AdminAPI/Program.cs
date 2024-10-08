using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Warehouse.Infrastructure;
using Warehouse.Infrastructure.Abstraction;
using Warehouse.Infrastructure.Extensions;
using Warehouse.Application.Extensions;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddRepositories();

        var config = builder.Configuration;
        builder.Services.AddDbContext(config);

        builder.Services.AddMediatR("Warehouse.AdminAPI");

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddRabbitMq();

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}