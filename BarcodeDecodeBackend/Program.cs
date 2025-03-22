using System.Reflection;
using BarcodeDecodeLib.Models.Dtos;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var appName = Assembly.GetExecutingAssembly().GetName().Name ?? "<NO NAME>";

var rabbitMqConfiguration = builder.Configuration.GetRequiredSection(nameof(RabbitMqConfig)).Get<RabbitMqConfig>()!;
builder.Services
    .AddMassTransit(cfg =>
    {
        cfg.AddOptions<MassTransitHostOptions>().Configure(opt => opt.WaitUntilStarted = true);
        cfg.SetEndpointNameFormatter(
            new DefaultEndpointNameFormatter(prefix: rabbitMqConfiguration!.EndpointNameFormatterPrefix, false));
        cfg.UsingRabbitMq((context, rabbitMqConfig) =>
        {
            rabbitMqConfig.Host(
                host: rabbitMqConfiguration.HostName,
                port: rabbitMqConfiguration.Port,
                virtualHost: rabbitMqConfiguration.VirtualHost,
                connectionName: appName,
                configure: cfg =>
                {
                    cfg.Username(rabbitMqConfiguration.Username);
                    cfg.Password(rabbitMqConfiguration.Password);
                });
            rabbitMqConfig.ConfigureEndpoints(context);
        });
    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();