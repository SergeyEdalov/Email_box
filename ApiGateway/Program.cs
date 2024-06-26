using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("totalAttectation.json")
                .Build();

            builder.Services.AddOcelot(configuration);
            builder.Services.AddSwaggerForOcelot(configuration);
            builder.WebHost.ConfigureKestrel((context, options) =>
            {
                options.ListenAnyIP(7052, listenOptions =>
                {
                    listenOptions.UseHttps("/app/aspnetapp.pfx", "Str0ngP@ssw0rd!");
                });
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            }).UseOcelot().Wait();

            app.UseHttpsRedirection();

            app.Run();
        }
    }
}
