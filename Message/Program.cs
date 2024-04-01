using Autofac.Extensions.DependencyInjection;
using Autofac;
using Message.Mapper;
using Message.Abstractions;
using Message.Services;
using Message.Database.Context;
using Microsoft.OpenApi.Models;

namespace Message
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAutoMapper(typeof(MessageMapper));
            builder.Services.AddSwaggerGen();

            var config = new ConfigurationBuilder();
            config.AddJsonFile("appsettings.json");
            var cfg = config.Build();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(cb =>
            {
                cb.Register(c => new MessageContext(cfg.GetConnectionString("db"))).InstancePerDependency();
            });

            builder.Services.AddTransient<IMessageService, MessageService>();

            var app = builder.Build();

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
}
