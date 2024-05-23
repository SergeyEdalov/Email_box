using Autofac.Extensions.DependencyInjection;
using Autofac;
using Message.Mapper;
using Message.Abstractions;
using Message.Services;
using Message.Database.Context;
using Microsoft.OpenApi.Models;
using Message.RabbitMq;
using RabbitMQ.Client;
using Microsoft.IdentityModel.Tokens;
using RSATools.RSAKeyFolder;

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

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Message API", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "Token",
                    Scheme = "bearer",
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(cb =>
            {
                cb.Register(c => new MessageContext(config.GetConnectionString("db"))).InstancePerDependency();
            });
            builder.Services.AddSingleton<IMessageService, MessageService>();

            builder.Services.AddSingleton<IConnectionFactory>(sp => new ConnectionFactory
            {
                HostName = "rabbitmq",
                DispatchConsumersAsync = true,
            });
            builder.Services.AddSingleton<IRabbitMqService<string, Guid>, RabbitMqService>();

            builder.Services.AddSingleton<RabbitMqListener>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<RabbitMqListener>());

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var authorizationHeader = context.Request.Headers["Authorization"].ToString();
                            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
                            {
                                context.Token = authorizationHeader.Substring("Bearer ".Length).Trim();
                            }
                            return Task.CompletedTask;
                        }
                    };
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new RsaSecurityKey(RsaToolsKeys.GetPrivateKey())
                    };
                });
            builder.Services.AddAuthorization();
            builder.WebHost.ConfigureKestrel((context, options) =>
            {
                options.ListenAnyIP(7191, listenOptions =>
                {
                    listenOptions.UseHttps("/app/aspnetapp.pfx", "Str0ngP@ssw0rd!");
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Message API V1");
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
