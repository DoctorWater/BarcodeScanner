using System.Reflection;
using BarcodeDecodeFrontend.Data.Services;
using BarcodeDecodeFrontend.Data.Services.Interfaces;
using BarcodeDecodeFrontend.Data.Services.Processing;
using BarcodeDecodeLib;
using BarcodeDecodeLib.Models.Dtos;
using Blazored.Modal;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var appName = Assembly.GetExecutingAssembly().GetName().Name ?? "<NO NAME>";
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredModal();

var rabbitMqConfiguration = builder.Configuration.GetRequiredSection(nameof(RabbitMqConfig)).Get<RabbitMqConfig>()!;

builder.Services.AddScoped<IBarcodeDecoder, BarcodeDecoder>()
    .AddScoped<IVideoProcessor, VideoProcessor>();
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();